using System;

namespace MemoryGame
{
    class Pictures
    {
        Random random; // Object of Random class

        public Pictures()
        {
            random = new Random(); // Initialize the Random object
        }

        // Function that shuffles the items of an array of strings
        internal string[] shuffleArray(string[] items)
        {
            int number; // Stores a random number
            string temp; // In this variable is stored the content of a specific position of the array

            for (int y = 0; y < items.Length; y++)
            {
                number = random.Next(23); // Generates a random number between 0 and 23

                //Change the content of the i position of the array with the content 
                //of a random's position's of the same array 
                temp = items[y];
                items[y] = items[number];
                items[number] = temp;
            }

            return items;
        }
    }
}
