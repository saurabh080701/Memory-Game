using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Media;

namespace MemoryGame
{
    public partial class GameArea : Form
    {
        Pictures picture; // Create a Picture object
        StringBuilder sb; // Object of kind StringBuilder 
        SoundPlayer player1,player2; // Objects of "SoundPlayer" class 

        string name; // Name of the user
        int tries = 0; // Number of tries that the user made during playing
        int time = 0; // Time in each round
        bool click1, click2 = false; // When these variables are both true, then the user has already clicked two pictureBoxes
        bool end = true; // When "end" variable is true, the game start's and when "end" variable is false, the game reset's

        // Declare an 24-sized array of type bool. This array shows the pictureBoxes that user clicked.
        // If flag[i]==true then the pictureBoxes[i] is clicked, and if flag[i] == false, that means that
        // the pictureBoxes[i] is not clicked.
        bool[] flag = new bool[24];

        // In this array, there will be saved all the paths of the default pictures (two times) needed for the game.
        // The pictures are 12 and the "items" array has 24 positions because we need PAIRS of pictures
        string[] items = new string[24];

        // This array will have the default images of the game (dinosaurs)
        string[] defaults = new string[24];

        PictureBox[] pictureBoxes; // Declare an array of PictureBox objects

        // Declare a list of type int. This list shows the positions of pictureBoxes (in the "pictureBoxes" array)
        // that user clicked.
        List<int> path = new List <int> ();

        // Declare a list of bool. When list's length is 24, the game will finish.
        // That means that each pictureBox is clicked, so the game finishes.
        List<bool> finish = new List<bool>();

        // In this list will be saved all the paths of the new Pictures that user wants.
        // The items of list "newPaths" are paths of pictures that are located in a 
        // directory that user chose.
        List<string> newPaths = new List<string>();

        // This array contains the paths of all the pictures from a specific directory that user chose
        string[] filePaths = new string[100];

        // In this list are stored all the paths of directory that user has already chosen.
        // If the pairs are not 12, then this list will be usefull for the programmer to determine
        // that new pictures will be added in the list "newPaths" only if the directory has not been
        // chosen by the user. So, for this reason i use this list "alreadyLooked"
        List<string> alreadyLooked = new List<string>();

        public GameArea(string n)
        {
            InitializeComponent();

            // Set name for the user
            name = n;
        }

        private void GameArea_Load(object sender, EventArgs e)
        {
            picture = new Pictures(); // Create an object of type Pictures

            // Initialize all the arrays
            for (int i = 0; i < 23; i++)
            {
                flag[i] = false; // flag array
            }

            // Add the same image 2 times to the "items" and "defaults" array (in order to have pairs)
            for (int i = 1; i <= 12; i++)
            {
                items[i-1] = "images/Dinosaur_" + i.ToString() + ".jpg";
                defaults[i-1] = "images/Dinosaur_" + i.ToString() + ".jpg";
            }

            for (int i = 1; i <= 12; i++) 
            {
                items[12+i-1] = "images/Dinosaur_" + i.ToString() + ".jpg";
                defaults[12 + i - 1] = "images/Dinosaur_" + i.ToString() + ".jpg";
            }

            // Array of PictureBoxes in which will be saved all the PictureBox objects (24 pictureBoxes)
            pictureBoxes = new PictureBox[24] { pictureBox1, pictureBox2, pictureBox3, pictureBox4, pictureBox5,
            pictureBox6, pictureBox7, pictureBox8, pictureBox9, pictureBox10,pictureBox11,pictureBox12,pictureBox13,
            pictureBox14,pictureBox15,pictureBox16,pictureBox17,pictureBox18,pictureBox19,pictureBox20,pictureBox21,
            pictureBox22,pictureBox23,pictureBox24};

            // sound that plays when user makes something wrong
            player1 = new SoundPlayer("error_sound.wav");

            // sound that plays when user finishes the game
            player2 = new SoundPlayer("finish_sound.wav");
        }

        // This function shuffles the "items", then gives a tag to each one of the 24 pictureBoxes.
        // Everytime that this function is excecuted, each pictureBox has another tag. In the tag property
        // of each pictureBox, is assigned a path of an image.
        void change(string[] array)
        {
            items = picture.shuffleArray(array); // Shuffle's the content of "items" array

            int index = 0; // Represent's the first position of an array
            foreach (PictureBox pictureBox in pictureBoxes)
            {
                // Set a random item of the array items as tag property of each PictureBox (24 pictureBoxes)
                pictureBox.Tag = items[index];

                index++; // Increase the index variable by 1
            }
        }

        // This function loads all the pictures from a specific directory that user chose.
        // The paths of these pictures are stored in the list "newPaths"
        void searchNewPictures(string path)
        {
            // Add to the array all the files with ".png" extention
            filePaths = Directory.GetFiles(path, "*.png", SearchOption.TopDirectoryOnly);

            foreach (string location in filePaths)
            {
                // Add the path of the picture two times (in order to have pairs)
                newPaths.Add(location);
                newPaths.Add(location);

                // Do not seach more if the items of the list "newPaths" are already 24
                if (newPaths.Count == 24)
                {
                    return;
                }
            }

            // Now look all the files with ".jpg" extention
            filePaths = Directory.GetFiles(path, "*.jpg", SearchOption.TopDirectoryOnly);

            foreach (string location in filePaths)
            {
                // Add the path of the picture two times (in order to have pairs)
                newPaths.Add(location);
                newPaths.Add(location);

                // Do not search more if the items of the list "newPaths" are already 24
                if (newPaths.Count == 24) 
                { 
                    return;
                }
            }

            // Now look all the files with ".bpm" extention
            filePaths = Directory.GetFiles(path, "*.bpm", SearchOption.TopDirectoryOnly);

            foreach (string location in filePaths)
            {
                // Add the path of the picture two times (in order to have pairs)
                newPaths.Add(path);
                newPaths.Add(path);

                // Do not seach more if the items of the list "newPaths" are already 24
                if (newPaths.Count == 24) 
                {
                    return;
                }
            }
        }

        // This function is executed when Users clicks the "Load Pictures" menu item:
        private void drawToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sb = new StringBuilder(); // Instantiate an object of kind StringBuilder

            newPaths.Clear(); // Clear all the "newPath" list
            alreadyLooked.Clear(); // Clear the "alreadyLooked" list

            // Look for new images while the list "newPaths" does not contain 24 items
            while (newPaths.Count < 24) 
            {
                sb.Append(newPaths.Count / 2 + " pictures have been loaded.");
                sb.Append(Environment.NewLine);
                sb.Append("Please select images from a directory.");
                
                MessageBox.Show(sb.ToString()); // Show a message to the user

                sb.Clear(); // Removes all the characters from the sb (StringBuilder) object
   
                // Dialog opens and user clicks OK button
                if (!(folderBrowserDialog1.ShowDialog() == DialogResult.Cancel)) 
                {
                    // Check if the path that user selected already exists in the list "alreadyLooked"
                    if (alreadyLooked.Contains(folderBrowserDialog1.SelectedPath) == false)
                    {
                        // Search pictures in the directory that user chose
                        searchNewPictures(folderBrowserDialog1.SelectedPath);

                        // Add the selected path to the "alreadyLooked" list
                        alreadyLooked.Add(folderBrowserDialog1.SelectedPath);
                    }
                    else
                    {
                        player1.Play(); // Play an "error" sound
                        MessageBox.Show("Please, select another path!");
                    }    
                } 
                
                else
                
                {
                    return; // If user presses "No" or "Cancel", then terminate this function
                }
            }

            MessageBox.Show("Transfer completed!");

            // Tranfer the content of the list "newPaths" to the "items" array
            for (int i = 0; i < 24; i++)
            {
                items[i] = newPaths[i];
            }

            change(items);
        }


        // This timer hides after some seconds the pictures, before the game begins
        private void timer3_Tick(object sender, EventArgs e)
        {
            for (int j = 0; j < 24; j++)
            {
                flag[j] = false;
                pictureBoxes[j].ImageLocation = "images/other_side.jpg";
            }

            finish.Clear(); // Clears all the elements of the list "finish"
            startToolStripMenuItem.Enabled = true;
            
            timer1.Enabled = true; // Enable the "timer1"

            end = false; // User can stop the game if he clicks in the "Reset" menu item
            
            timer3.Enabled = false; // Disable "timer3"
        }

        // This code will be excecuted when the "Start" menu is clicked:
        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (end == true) // Game starts (start operation)
            {
                timer1.Enabled = false;
                startToolStripMenuItem.Text = "Reset"; // Change the text of the button
                
                startToolStripMenuItem.Enabled = false;
                toolStripMenuItem2.Enabled = false;
                drawToolStripMenuItem.Enabled = false;

                // Mark as false these two variables
                click1 = false;
                click2 = false;

                path.Clear(); // Clear all the elements of "path" list

                label1.Text = "0"; // Set tries to 0
                label3.Text = "0"; // Set the time to 0

                // Initiallize again the "tries" and "time" variables
                tries = 0;
                time = 0;

                change(items); // Change ranbomly the content of each pictureBox

                // Show the images to the user before the game starts
                for (int i = 0; i < 24; i++)
                {
                    //startToolStripMenuItem.Enabled = false; // The user can not press the start button
                    pictureBoxes[i].ImageLocation = pictureBoxes[i].Tag.ToString();
                }

                timer3.Enabled = true; // Enable timer3
            } 
            else // User can stop the game (reset operation)
            {
                timer1.Enabled = false; // Stop the game

                // Load the default images of the game, if new images have been loaded by the user
                if (newPaths.Count <= 24)
                {
                    for (int m = 0; m < 24; m++)
                    {
                        items[m] = defaults[m];
                    }
                }

                startToolStripMenuItem.Text = "Start"; // Change the name of the "start" menu

                label1.Text = "0"; // Set tries to 0
                label3.Text = "0"; // Set the time to 0

                // Turn all the images upside-down
                for (int j = 0; j < 24; j++)
                {
                    flag[j] = false;
                    pictureBoxes[j].ImageLocation = "images/other_side.jpg";
                }

                // User can click all the menu items
                toolStripMenuItem2.Enabled = true;
                drawToolStripMenuItem.Enabled = true;

                end = true; // User can play if he presses the "Start" menu item
            }
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            // Create an object of type Top10
            Top10 top = new Top10();
            top.ShowDialog(); // Shows the form "Results"
        }


        // This function is excecuted when timer1 is enabled
        private void timer1_Tick(object sender, EventArgs e)
        {
            time++; // Increase time by 1
            label3.Text = time.ToString(); // Set time to the label1

            if (finish.Count == 24) // Game is over
            {
                timer1.Enabled = false; // Stop the timer1 when the game finishes
                
                player2.Play(); // Play a sound when the game finishes

                MessageBox.Show("Congratulations!"); // Show a message to the user

                // Add (or Update) to the DB the characteristics of the user who plays
                new User().databaseOperations(name, tries, time); 

                // Create an object of type Results
                Results results = new Results(name, tries.ToString(), time.ToString());
                results.ShowDialog(); // Shows the form "Results"

                startToolStripMenuItem.Text = "Start"; // Change the text of menu from "Reset" to "Start"               
                label3.Text = "0"; // Set the "time" to 0
                label1.Text = "0"; // Set "tries" to 0

                // Initiallize again the "tries" and "time" variables
                tries = 0;
                time = 0;

                end = true; // User can play again if he clicks the "Start" menu item

                toolStripMenuItem2.Enabled = true;
                drawToolStripMenuItem.Enabled = true;

                // Upside down all the images
                foreach (PictureBox pic in pictureBoxes)
                {
                    pic.ImageLocation = "images/other_side.jpg";
                }

                finish.Clear(); // Clears all the elements in the list "finish"
                
            }
        }

        void onClickOperation(int number)
        {
            if (timer1.Enabled == true && flag[number-1]==false)
            {
                flag[number - 1] = true; // mark as "true" a specific element of array flag
                pictureBoxes[number - 1].ImageLocation = pictureBoxes[number - 1].Tag.ToString();
                path.Add(number - 1); // Add n-1 to the list

                // This if..else condition shows when the user has already clicked two pictureBoxes
                // So, if click1 and click2 are true, that means that the user has clicked two 
                // pictureBoxes. So, the program needs to examine if these two pictures are the same.
                // After this control, "click1" and "click2" variables are setted to false. That means
                // that no pictureBox have been clicked by the user.
                if (click1 == false)
                {
                    click1 = true;
                    click2 = false;
                }
                else if (click2 == false)
                {
                    click1 = true;
                    click2 = true;
                }

                if (click1 == true && click2 == true && path.Count == 2) // Two pictureBoxes have been clicked
                {
                    click1 = click2 = false; // That means that no pictureBox have been clicked 

                    // Check if images are the same
                    if (pictureBoxes[path[0]].ImageLocation.Equals(pictureBoxes[path[1]].ImageLocation)) 
                    {
                        tries += 1; // Increase "tries" variable by one
                        label1.Text = tries.ToString(); // Update label1 (shows the number of tries user made in total)

                        // The images are the same, so remove all the elements from the list
                        path.Clear();

                        // Add two items in the list "finish"
                        finish.Add(true);
                        finish.Add(true);
                    }

                    else // The images are not the same

                    {
                        tries += 1; // Increase "tries" variable by one
                        label1.Text = tries.ToString(); // Update label1 (shows the number of tries user made in total)

                        // Set flag variables to false
                        flag[path[0]] = false;
                        flag[path[1]] = false;

                        // Turn the images upside down (for this reason use timer)
                        timer2.Enabled = true; // Enable timer2
                    }
                    
                }
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            pictureBoxes[path[0]].ImageLocation = "images/other_side.jpg";
            pictureBoxes[path[1]].ImageLocation = "images/other_side.jpg";

            // Remove all the elements from the list
            path.Clear();

            timer2.Enabled = false; // Disable timer
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            onClickOperation(1);
        }

        private void pictureBox2_MouseClick(object sender, MouseEventArgs e)
        {
            onClickOperation(2);
        }

        private void pictureBox3_MouseClick(object sender, MouseEventArgs e)
        {
            onClickOperation(3);
        }

        private void pictureBox4_MouseClick(object sender, MouseEventArgs e)
        {
            onClickOperation(4);
        }

        private void pictureBox5_MouseClick(object sender, MouseEventArgs e)
        {
            onClickOperation(5);
        }

        private void pictureBox6_MouseClick(object sender, MouseEventArgs e)
        {
            onClickOperation(6);
        }

        private void pictureBox7_MouseClick(object sender, MouseEventArgs e)
        {
            onClickOperation(7);
        }

        private void pictureBox8_MouseClick(object sender, MouseEventArgs e)
        {
            onClickOperation(8);
        }

        private void pictureBox9_MouseClick(object sender, MouseEventArgs e)
        {
            onClickOperation(9);
        }

        private void pictureBox10_MouseClick(object sender, MouseEventArgs e)
        {
            onClickOperation(10);
        }

        private void pictureBox11_MouseClick(object sender, MouseEventArgs e)
        {
            onClickOperation(11);
        }

        private void pictureBox12_MouseClick(object sender, MouseEventArgs e)
        {
            onClickOperation(12);
        }

        private void pictureBox13_MouseClick(object sender, MouseEventArgs e)
        {
            onClickOperation(13);
        }

        private void pictureBox14_MouseClick(object sender, MouseEventArgs e)
        {
            onClickOperation(14);
        }

        private void pictureBox15_MouseClick(object sender, MouseEventArgs e)
        {
            onClickOperation(15);
        }

        private void pictureBox16_MouseClick(object sender, MouseEventArgs e)
        {
            onClickOperation(16);
        }

        private void pictureBox17_MouseClick(object sender, MouseEventArgs e)
        {
            onClickOperation(17);
        }

        private void pictureBox18_MouseClick(object sender, MouseEventArgs e)
        {
            onClickOperation(18);
        }

        private void pictureBox19_MouseClick(object sender, MouseEventArgs e)
        {
            onClickOperation(19);
        }

        private void pictureBox20_MouseClick(object sender, MouseEventArgs e)
        {
            onClickOperation(20);
        }

        private void pictureBox21_MouseClick(object sender, MouseEventArgs e)
        {
            onClickOperation(21);
        }

        private void pictureBox22_MouseClick(object sender, MouseEventArgs e)
        {
            onClickOperation(22);
        }

        private void folderBrowserDialog2_HelpRequest(object sender, EventArgs e)
        {

        }

        private void pictureBox23_MouseClick(object sender, MouseEventArgs e)
        {
            onClickOperation(23);
        }

        private void pictureBox24_MouseClick(object sender, MouseEventArgs e)
        {
            onClickOperation(24);
        }
    }
}
