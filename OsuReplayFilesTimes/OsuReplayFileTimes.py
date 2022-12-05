import os
import csv
import datetime
import matplotlib.pyplot as plt

# directory to get the last modified time for each file in
directory = 'C:/Users/Regex/AppData/Local/osu!/Data/r/'

# create a dictionary to store the last modified hour for each file
last_modified_hours = {}

# iterate over the files in the directory
for filename in os.listdir(directory):
    # only consider files with the .osr file extension
    if not filename.endswith('.osr'):
        continue
    
    # get the full path of the file
    filepath = os.path.join(directory, filename)
    
    # get the last modified time for the file
    last_modified_time = datetime.datetime.fromtimestamp(os.path.getmtime(filepath))

    # get the last modified hour for the file
    last_modified_hour = last_modified_time.strftime('%H')

    # add the last modified hour to the dictionary
    if last_modified_hour in last_modified_hours:
        last_modified_hours[last_modified_hour] += 1
    else:
        last_modified_hours[last_modified_hour] = 1

# create a scatterplot of the last modified hours
hours = sorted(list(last_modified_hours.keys()))
counts = [last_modified_hours[hour] for hour in hours]
plt.scatter(hours, counts)

# add labels to the axes
plt.xlabel('Last modified hour')
plt.ylabel('Number of files')

# show the scatterplot
plt.show()
