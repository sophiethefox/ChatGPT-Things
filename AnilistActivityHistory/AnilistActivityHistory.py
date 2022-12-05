import json
from datetime import datetime
import matplotlib.pyplot as plt
from matplotlib.dates import WeekdayLocator

# load the JSON data from the file
with open("anilist_data.json") as json_file:
    data = json.load(json_file)

# initialize a dictionary to store the counts for each day
activity_counts = {}

# iterate over the activity array in the JSON data
for activity in data["activity"]:
    # get the "created_at" timestamp and parse it into a datetime object
    created_at = datetime.strptime(activity["created_at"], "%Y-%m-%d %H:%M:%S")

    # get the date part of the datetime object
    date = created_at.date()

    # update the activity counts dictionary
    if date not in activity_counts:
        activity_counts[date] = 1
    else:
        activity_counts[date] += 1

# sort the dates and counts
dates = sorted(activity_counts.keys())
counts = [activity_counts[date] for date in dates]

# set the x-axis tick locations to be every Monday
plt.gca().xaxis.set_major_locator(WeekdayLocator(byweekday=0))

# plot the data
plt.plot(dates, counts)
plt.show()