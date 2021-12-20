import logging
import statistics
from enum import Enum

import matplotlib.pyplot as plt
from matplotlib.widgets import Button
import numpy as np
import colorlog


def logger_config(name=None):
    """Setup the logging environment."""
    if not name:
        log = logging.getLogger()  # root logger
    else:
        log = logging.getLogger(name)
    log.setLevel(logging.DEBUG)
    format_str = "%(asctime)s.%(msecs)d %(levelname)-8s [%(filename)s:%(lineno)d] %(message)s"
    date_format = "%Y-%m-%d %H:%M:%S"
    cformat = "%(log_color)s" + format_str
    colors = {
        "DEBUG": "green",
        "INFO": "white",
        "WARNING": "bold_yellow",
        "ERROR": "bold_red",
        "CRITICAL": "bold_purple",
    }
    formatter = colorlog.ColoredFormatter(cformat, date_format, log_colors=colors)
    stream_handler = logging.StreamHandler()
    stream_handler.setFormatter(formatter)
    log.addHandler(stream_handler)
    return log


STARTING_X = 0
STARTING_Y = 0
MAIN_PATH_MIN_LENGTH = 65
MAIN_PATH_MAX_LENGTH = 80
BRANCH_MAX_LENGTH = 6
BRANCH_MIN_LENGTH = 4
MIN_ROOM_WIDTH = 3
MAX_ROOM_WIDTH = 5
MIN_ROOM_HEIGHT = 3
MAX_ROOM_HEIGHT = 5
REMOVE_ROOM_CORNER_CHANCE = 30
DISTANCE_UNTIL_TURN_MEAN = 6
DISTANCE_UNTIL_TURN_STD_DEV = 1
DISTANCE_UNTIL_TURN_MIN = 2
BRANCH_AT_DIRECTION_CHANGE_CHANCE = 50
ROOM_AT_DIRECTION_CHANGE_CHANCE = 10
SHIFT_CHANCE = 20


class Direction(Enum):
    UP = 0
    DOWN = 1
    LEFT = 2
    RIGHT = 3


index_deltas = {
    Direction.UP: (0, -1),
    Direction.DOWN: (0, 1),
    Direction.LEFT: (-1, 0),
    Direction.RIGHT: (1, 0),
}

DEFAULT_DIRECTION_WEIGHT = 1
PRIORITY_DIRECTION_WEIGHT = 3
opposite_direction = {
    Direction.UP: Direction.DOWN,
    Direction.DOWN: Direction.UP,
    Direction.LEFT: Direction.RIGHT,
    Direction.RIGHT: Direction.LEFT,
}
change_direction_weights = {
    Direction.UP: DEFAULT_DIRECTION_WEIGHT,
    Direction.DOWN: DEFAULT_DIRECTION_WEIGHT,
    Direction.LEFT: DEFAULT_DIRECTION_WEIGHT,
    Direction.RIGHT: DEFAULT_DIRECTION_WEIGHT,
}
branch_direction_weights = {
    Direction.UP: DEFAULT_DIRECTION_WEIGHT,
    Direction.DOWN: DEFAULT_DIRECTION_WEIGHT,
    Direction.LEFT: DEFAULT_DIRECTION_WEIGHT,
    Direction.RIGHT: DEFAULT_DIRECTION_WEIGHT,
}
turn_90_deg_options = {
    Direction.UP: [Direction.LEFT, Direction.RIGHT],
    Direction.DOWN: [Direction.LEFT, Direction.RIGHT],
    Direction.LEFT: [Direction.UP, Direction.DOWN],
    Direction.RIGHT: [Direction.UP, Direction.DOWN],
}


class GridSquare:
    def __init__(self, x: int, y: int):
        self.x = x
        self.y = y


def percent_chance(percent: int) -> bool:
    return np.random.randint(100) < percent


def gaussian(mean: int, std_dev: int, low_limit: int = -999, up_limit: int = 999) -> int:
    val = 0
    for _ in range(12):
        val += np.random.random()
    val -= 6
    val = mean + (val * std_dev)
    ret = int(np.round(val))
    return np.clip(ret, low_limit, up_limit)


def choose_weighted(my_list, weights: dict):
    total_weights = 0
    for item in my_list:
        total_weights += weights[item]
    rand = np.random.randint(0, total_weights)
    check = 0
    ret = None
    for item in my_list:
        check += weights[item]
        if rand < check:
            ret = item
            break
    return ret


class MapGenerator:
    def __init__(self):
        self.clean()
        self.distance_until_direction_change = 0

    def clean(self):
        self.grid_squares = []
        self.seed = np.random.randint(0, 2 ** 31)
        np.random.seed(self.seed)

    def plot(self):
        fig, ax = plt.subplots()

        def prepare_map() -> np.ndarray:
            max_x = self._get_max_x()
            min_x = self._get_min_x()
            max_y = self._get_max_y()
            min_y = self._get_min_y()
            logger.debug(f"max X: {max_x} min X: {min_x} max Y: {max_y} min Y: {min_y}")

            EDGE_BUFFER = 2
            width = (max_x - min_x) + 1 + (2 * EDGE_BUFFER)
            height = (max_y - min_y) + 1 + (2 * EDGE_BUFFER)

            extra_in_height = 0
            extra_in_width = 0
            if width > height:
                extra_in_height = (width - height) // 2
            elif height > width:
                extra_in_width = (height - width) // 2
            grid_size = max(width, height)

            zvals = np.zeros((grid_size, grid_size))
            for grid_square in self.grid_squares:
                x_idx = grid_square.y - min_y + EDGE_BUFFER + extra_in_height
                y_idx = grid_square.x - min_x + EDGE_BUFFER + extra_in_width
                zvals[x_idx][y_idx] = 1
            x_idx = STARTING_Y - min_y + EDGE_BUFFER + extra_in_height
            y_idx = STARTING_X - min_x + EDGE_BUFFER + extra_in_width
            zvals[x_idx][y_idx] = 2
            return zvals

        def update_title():
            ax.set_title(f"Seed {self.seed} (floor area = {len(self.grid_squares)})")

        def new_map(event):
            logger.debug(f"Event {event}")
            logger.info("Generating new map")
            self.clean()
            self.generate()
            zvals = prepare_map()
            update_title()
            im.set_data(zvals)
            fig.canvas.draw_idle()

        zvals = prepare_map()
        im = ax.imshow(zvals)
        update_title()
        BUTTON_SIZE = 0.1
        axnew = plt.axes([0.85, 0.45, BUTTON_SIZE, BUTTON_SIZE])
        bnew = Button(axnew, "New")
        bnew.on_clicked(new_map)

        plt.show()

    def generate(self):
        current_grid_square = GridSquare(STARTING_X, STARTING_Y)
        self.grid_squares.append(current_grid_square)
        self._create_room(current_grid_square)
        current_direction = np.random.choice(list(Direction))
        logger.info(f"Current direction: {current_direction}")
        self._update_direction_weights(current_direction)
        main_path_length = np.random.randint(MAIN_PATH_MIN_LENGTH, MAIN_PATH_MAX_LENGTH + 1)
        self.distance_until_direction_change = self._new_distance_until_direction_change()
        logger.info(f"Distance until direction change is {self.distance_until_direction_change}")

        for _ in range(main_path_length):
            if self.distance_until_direction_change == 0:
                current_direction = self._change_direction(current_grid_square, current_direction)

            if percent_chance(SHIFT_CHANCE):
                current_grid_square = self._shift_sideways_one(current_grid_square, current_direction)
            current_grid_square = self._create_gridsquare_in_direction(current_grid_square, current_direction)
            self.distance_until_direction_change -= 1
        self._create_room(current_grid_square)

    def statistics(self, num_generations: int = 1000):
        logger.disabled = True
        floor_areas = []
        PRINT_EVERY_PERCENT = 0.1
        next_print_at_percent = PRINT_EVERY_PERCENT
        for idx in range(num_generations):
            self.clean()
            self.generate()
            floor_areas.append(len(self.grid_squares))
            progress = 100 * idx / num_generations
            if progress >= next_print_at_percent:
                next_print_at_percent += PRINT_EVERY_PERCENT
                print(f"Statistics: {progress:.2f}% complete")
        _, (ax1) = plt.subplots(1, sharex=True)
        print(f"Mean: {statistics.mean(floor_areas)}")
        print(f"mode: {statistics.mode(floor_areas)}")
        print(f"median: {statistics.median(floor_areas)}")
        print(f"standard deviation: {statistics.stdev(floor_areas)}")
        ax1.grid(zorder=0)
        ax1.hist(floor_areas, color="blue", edgecolor="black", bins=(max(floor_areas) - min(floor_areas)), zorder=3)
        ax1.set_title(f"Level Generation Floor Area ({len(floor_areas)} Generations)")
        ax1.set_ylabel("Number of Occurrences")
        ax1.set_xlabel("Floor Area")
        logger.disabled = False
        plt.show()

    def _change_direction(self, current_gridsquare: GridSquare, current_direction: Direction):
        previous_direction = current_direction
        new_direction = self._choose_new_direction(current_direction)
        logger.info(f"New direction is {new_direction}")
        self.distance_until_direction_change = self._new_distance_until_direction_change()
        logger.info(f"New distance until direction change is {self.distance_until_direction_change}")
        if percent_chance(BRANCH_AT_DIRECTION_CHANGE_CHANCE):
            options = [previous_direction, opposite_direction[new_direction]]
            self._create_branch(current_gridsquare, choose_weighted(options, branch_direction_weights))
        if percent_chance(ROOM_AT_DIRECTION_CHANGE_CHANCE):
            self._create_room(current_gridsquare)
        return new_direction

    def _create_branch(self, start: GridSquare, branch_direction: Direction):
        logger.info(f"Creating branch at [{start.x}, {start.y}] in direction {branch_direction}")
        branch_current_gridsquare = start
        branch_length = np.random.randint(BRANCH_MIN_LENGTH, BRANCH_MAX_LENGTH + 1)
        for _ in range(branch_length):
            branch_current_gridsquare = self._create_gridsquare_in_direction(
                branch_current_gridsquare, branch_direction
            )
        self._create_room(branch_current_gridsquare)

    def _new_distance_until_direction_change(self) -> int:
        return gaussian(DISTANCE_UNTIL_TURN_MEAN, DISTANCE_UNTIL_TURN_STD_DEV, DISTANCE_UNTIL_TURN_MIN)

    def _choose_new_direction(self, current_direction: Direction) -> Direction:
        return choose_weighted(turn_90_deg_options[current_direction], change_direction_weights)

    def _create_room(
        self,
        center: GridSquare,
        min_w: int = MIN_ROOM_WIDTH,
        max_w: int = MAX_ROOM_WIDTH,
        min_h: int = MIN_ROOM_HEIGHT,
        max_h: int = MAX_ROOM_HEIGHT,
    ):
        logger.info(f"Creating room at [{center.x}, {center.y}]")
        room_width = np.random.randint(min_w, max_w + 1)
        room_height = np.random.randint(min_h, max_h + 1)

        logger.debug(f"The room width is {room_width}")
        logger.debug(f"The room height is {room_height}")
        origin_x = center.x + 1 - ((room_width + 1) // 2)
        origin_y = center.y + 1 - ((room_height + 1) // 2)

        for x_idx in range(room_width):
            for y_idx in range(room_height):
                corner = (x_idx == 0 or x_idx == room_width - 1) and (y_idx == 0 or y_idx == room_height - 1)
                if not (corner and percent_chance(REMOVE_ROOM_CORNER_CHANCE)):
                    self._try_create_new_grid_square(origin_x + x_idx, origin_y + y_idx)
                else:
                    logger.debug(f"Room corner at [{x_idx}, {y_idx}] ignored")

    def _update_direction_weights(self, current_direction: Direction):
        logger.info("Updating the direction weights")
        weighted_direction_1 = current_direction
        logger.debug(f"The 90 deg. options for {current_direction} are {turn_90_deg_options[current_direction]}")
        weighted_direction_2 = np.random.choice(turn_90_deg_options[current_direction])
        logger.debug(f"{weighted_direction_2} was chosen")
        change_direction_weights[weighted_direction_1] = PRIORITY_DIRECTION_WEIGHT
        change_direction_weights[weighted_direction_2] = PRIORITY_DIRECTION_WEIGHT
        branch_direction_weights[weighted_direction_1] = PRIORITY_DIRECTION_WEIGHT
        branch_direction_weights[weighted_direction_2] = PRIORITY_DIRECTION_WEIGHT
        logger.debug(f"change_direction_weights are now {change_direction_weights}")
        logger.debug(f"branch_direction_weights are now {branch_direction_weights}")

    def _shift_sideways_one(self, current_grid_square: GridSquare, current_direction: Direction) -> GridSquare:
        return self._create_gridsquare_in_direction(
            current_grid_square, np.random.choice(turn_90_deg_options[current_direction])
        )

    def _create_gridsquare_in_direction(self, current: GridSquare, direction: Direction) -> GridSquare:
        idx_delta = index_deltas[direction]
        new_x_idx = current.x + idx_delta[0]
        new_y_idx = current.y + idx_delta[1]
        return self._try_create_new_grid_square(new_x_idx, new_y_idx)

    def _try_create_new_grid_square(self, x_idx: int, y_idx: int) -> GridSquare:
        grid_square_exists = next((gs for gs in self.grid_squares if (gs.x == x_idx and gs.y == y_idx)), None)
        if grid_square_exists is None:
            logger.debug(f"Creating new grid square at [{x_idx}, {y_idx}]")
            new_grid_square = GridSquare(x_idx, y_idx)
            self.grid_squares.append(new_grid_square)
            return new_grid_square
        else:
            logger.debug(f"Grid square at [{x_idx}, {y_idx}] already exists")
            return grid_square_exists

    def _get_max_x(self) -> int:
        return max(grid_square.x for grid_square in self.grid_squares)

    def _get_max_y(self) -> int:
        return max(grid_square.y for grid_square in self.grid_squares)

    def _get_min_x(self) -> int:
        return min(grid_square.x for grid_square in self.grid_squares)

    def _get_min_y(self) -> int:
        return min(grid_square.y for grid_square in self.grid_squares)


def main():
    global logger
    logger = logger_config("my_logger")

    map_generator = MapGenerator()
    # map_generator.statistics(num_generations=10000)
    map_generator.generate()
    map_generator.plot()


if __name__ == "__main__":
    main()
