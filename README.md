# Random Flashcards

A simple program which randomly selects random values from a CSV file in a flashy way.

Double click on the program to begin randomly cycling through the elements within the selected category. If you include a file named `sound effect.wav` then that will play whenever you initiate the random cycling. Any other audio file will be ignored.

The header of `Card List.CSV` sets the different categories. If the name of a category is appeded with `;delete` then the values in that category will be removed from the pool of available options when the program lands on them. The values are only removes from working memory, the program will not write any changes to the CSV file.
