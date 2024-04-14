from matplotlib import pyplot as plt
with open('time_values.txt', 'r') as f:
    values = [float(el.strip().replace(',', '.')) for el in f]

plt.hist(values, bins=100)
plt.savefig('hist.png')
plt.clf()

plt.hist(values, bins=100, density=True, cumulative=True)
plt.savefig('cdf.png')
