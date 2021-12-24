import numpy as np
import matplotlib.pyplot as plt
import random
import statistics

def plot_standard_deviations():
    # References: https://en.wikipedia.org/wiki/Normal_distribution#Computational_methods

    def convert_std_normal_to_normal(mean: float, std_dev: float, std_normal):
        return mean + std_dev * std_normal

    STANDARD_DEVIATION = 1.0
    MEAN = 5.0

    fig, (ax1, ax2, ax3) = plt.subplots(3, sharex=True)
    fig.suptitle('Computationally Generating Normal Distruted Values')

    # =====================================
    # Normal Distribution Function
    # =====================================

    x = np.linspace(0, 20, 100)
    f1 = 1 / (STANDARD_DEVIATION * np.sqrt(2 * np.pi))
    exponent = -0.5 * np.square((x - MEAN) / STANDARD_DEVIATION)
    f2 = np.power(np.e, exponent)
    f = f1 * f2
    ax1.plot(x, f)
    ax1.set_title("Normal Distribution Function")
    ax1.set_ylabel("Probability Density f(x)")
    ax1.set_xlabel("x")

    # =====================================
    # Irwin-Hall Method
    # =====================================

    REPEATS = 100000
    results = []
    for _ in range(REPEATS):
        val = 0
        for x in range(12):
            val += random.random()
        val -= 6
        val = convert_std_normal_to_normal(MEAN, STANDARD_DEVIATION, val)
        results.append(val)
    ax2.hist(results, color = 'blue', edgecolor = 'black', bins=(1000))
    ax2.set_title(f"Irwin-Hall Method ({REPEATS} Repeats)")
    ax2.set_ylabel("Number of x")
    ax2.set_xlabel("x")

    # =====================================
    # Box-Muller Method
    # =====================================

    results = []
    REPEATS = 100000
    for _ in range(REPEATS):
        u = 1.0 - random.random()
        v = 1.0 - random.random()
        val = np.sqrt(-2.0 * np.log(u)) * np.sin(2.0 * np.pi * v)
        val = convert_std_normal_to_normal(MEAN, STANDARD_DEVIATION, val)
        results.append(val)
    ax3.hist(results, color = 'blue', edgecolor = 'black', bins=(100))
    ax3.set_title(f"Box-Muller Method ({REPEATS} Repeats)")
    ax3.set_ylabel("Number of x")
    ax3.set_xlabel("x")



def plot_room_simulations():
    fig, (ax1) = plt.subplots(1, sharex=True)
    fig.suptitle('')
    with open("data.txt", "r") as filestream:
        for line in filestream:
            parts = line.split(",")
            break
    data = [int(x) for x in parts[:-1]]
    print(f"Mean: {statistics.mean(data)}, mode: {statistics.mode(data)}, median: {statistics.median(data)}, standard deviation: {statistics.stdev(data)}")
    ax1.grid(zorder=0)
    ax1.hist(data, color = 'blue', edgecolor = 'black', bins=(max(data) - min(data)), zorder=3)
    ax1.set_title(f"Level Generation Floor Area ({len(data)} Generations)")
    ax1.set_ylabel("Number of Occurrences")
    ax1.set_xlabel("Floor Area")

if __name__ == "__main__":
    plot_standard_deviations()
    plot_room_simulations()
    
    plt.show()

"""
        private void GenerateFloorTest()
        {
            string path = @"D:\Git Projects\dungeon-crawler\scripts\data.txt";
            File.WriteAllText(path, string.Empty);

            using (StreamWriter w = File.AppendText(path))
            {
                for (int idx = 0; idx < 1000000; idx++)
                {
                    _gridSquares.Clear();
                    GenerateFloor();
                    string gridSquareCount = _gridSquares.Count.ToString() + ",";
                    w.Write(gridSquareCount);
                }
            }
        }
"""