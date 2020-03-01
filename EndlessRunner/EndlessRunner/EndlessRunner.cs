/* Author:          Steven Ma
 * Filename:        EndlessRunner.cs
 * Project Name:    EndlessRunner
 * Creation Date:   April 8, 2018
 * Modified Date:   April 22, 2018
 * Description:     Creating an Endless Runner that contains 3 obstables, 3 bonus
 *                  score items, and scrolls the screen every 20 seconds to a max
 *                  of 3x the original speed.
 *                  (c) Copyright Steven Ma. All Rights Reserved
 */
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace EndlessRunner
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    ///
    public class EndlessRunner : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //Stores the global rate of change for animation
        const int MILISECONDS_PER_FRAME = 50;

        //Creates Random Object
        Random rng = new Random();

        //Declares the Title Font
        SpriteFont titleFont;
        SpriteFont gameFont;

        //Creates and stores the game states
        enum GameState { Start, InGame, GameOver, Controls };
        GameState currentGameState = GameState.Start;

        //Stores the player's default values
        Texture2D jackieChan;
        Vector2 jackieChanPosition;
        int currentFrame = 0;
        Point frameSize = new Point((int)27.5, 50);
        const int PLAYER_SHEET_SIZE = 6;
        Rectangle animJackie;
        Rectangle jackieCollide;
        int minimumHeight;
        int timeSinceLastFrameJC = 0;

        //Stores the player's roll values
        Texture2D jackieChanRoll;
        int rollCurrentFrame = 0;
        Point rollFrameSize = new Point(40, 50);
        const int ROLL_SHEET_SIZE = 4;
        int rollOffset;
        Rectangle rollJackie;
        //Roll Animation Speed
        int rollTimeSinceLastFrame;

        //Stores the player's duck values
        Texture2D jackieChanDuck;
        const int DUCK_SHEET_SIZE = 3;
        Point duckFrameSize = new Point(44, 63);
        int duckCurrentFrame = 0;
        Point duckCollision = new Point(40, 32);
        Rectangle duckJackie;

        //Stores the player's death values
        bool isHeDead;
        Texture2D jackieChanDeath;
        int deathCurrentFrame = 0;
        Point deathFrameSize = new Point(48, 52);
        Rectangle deathJackie;

        //Stores the box values
        Texture2D box;
        Rectangle boxParameter;

        //Stores box crash values
        Texture2D boxCrash;
        bool isCrash;
        Rectangle boxCrashRec;
        Point crashFrameSize = new Point(68, 55);
        int crashCurrentFrame;
        const int CRASH_SHEET_SIZE = 6;
        int timeSinceLastFrameCrash;

        //Stores the grass obstacle's values
        Texture2D lowGrass;
        Rectangle grassRectangle;

        //Stores the gangster's values
        Texture2D gangster;
        Rectangle gangsterRec;

        //Stores the gangster death values
        bool isGangsterHit;
        Texture2D gangsterDie;
        Rectangle gangsterDeathRec;

        //Stores background values
        Texture2D background;
        Rectangle bgLoc;
        Rectangle bgLoc2;

        //Stores the bus values
        Texture2D bus;
        Rectangle busLoc;

        //Star Animation
        Point redStarFrameSize = new Point(22, 24);
        int starCurrentFrame = 0;
        const int STAR_SHEET_SIZE = 8;
        int starOffset = 14;
        int timeSinceLastFrameStar;
        Rectangle starAnim;
        Rectangle starCollide;

        //Stores the star' sprite sheets
        Texture2D orangeStar;
        Texture2D redStar;
        Texture2D greenStar;
        bool[] isStarsVisible = new bool[3];

        //Stores the scores and its Value
        int trackedTimeSurvived;
        int currentScore;
        int[] starQuantity = new int[3];
        int[] scoreItems = new int[] { 10, 15, 25, 3 };
        int highScore;

        //Stores the values of the backdrop for the scores
        Texture2D itemBackDrop;
        Rectangle itemBackDropLoc;

        //Stores
        int speedGame = 10;
        const int SPEED_BUS = 10;

        //Stores whether player ducks
        bool isDuck;

        //Stores the projectile values for Player and gangster
        const float INITIAL_VELOCITY = 4.33f;
        Vector2 gravity = new Vector2(0, (float)(-9.81 / 60));
        Vector2 playerTrajectory = new Vector2(0, INITIAL_VELOCITY);
        Vector2 deathProjectile = new Vector2(2.5f, INITIAL_VELOCITY);
        Vector2 gangsterTrajectory = new Vector2(0, INITIAL_VELOCITY);

        //Screen Size
        int screenWidth = 0;
        int screenHeight = 0;

        //Stores Keyboard State
        KeyboardState kb;
        KeyboardState prevKb;

        //Stores mouse state
        MouseState prevMouse;
        MouseState mouse;

        //Stores main menu item values
        Texture2D startButton;
        Rectangle startBtnLoc;
        Texture2D quitButton;
        Rectangle quitBtnLoc;
        Texture2D controlsBtn;
        Rectangle controlsBtnLoc;

        //Stores control menu values
        Texture2D controls;
        Rectangle controlsLoc;
        Texture2D collectableGuide;
        Rectangle collectableGuideLoc;
        Texture2D escapeKey;
        Rectangle escapeKeyLoc;

        //Stores the Game Over Options
        Texture2D retryBtn;
        Rectangle retryBtnLoc;
        Texture2D menuBtn;
        Rectangle menuBtnLoc;

        //Stores location of texts
        Vector2 highScoreLoc = new Vector2(700, 100);
        Vector2 currentScoreLoc = new Vector2(700, 20);
        Vector2 greenStarTxtLoc = new Vector2(700, 92);
        Vector2 redStarTxtLoc = new Vector2(700, 62);
        Vector2 orangeStarTxtLoc = new Vector2(700, 32);
        Vector2 starsTxtLoc = new Vector2(700, 2);
        Vector2 inGameScoreTxtLoc = new Vector2(700, 152);



        //Stores the chances for which star is to spawn
        int starRandom;

        //Time Track for Object spawn
        int timeTracked;
        bool isRandomized;

        //Stores whether obstacles are spawned
        bool[] isObstaclesOn = new bool[3];

        //Stores randomly generated number to spawn new obstacles
        int rollDie;
        int isTimeTracked = 1;
        int timeBeforeSpawn = 1000;

        //Stores the time before spped is increased
        const int TIME_BEFORE_SPEED_INCREASE = 20000;
        int speedTimeTrack;
        int isSpeedTracked = 1;
        int counterSpeed;
        const int MAXIMUM_SPEED_INCREASE = 3;

        //Stores background music
        Song inGameMusic;
        Song gameOverMusic;
        Song mainMenuMusic;

        //Stores sound effects
        SoundEffect hkPunch;
        SoundEffectInstance hkPunchInstance;
        SoundEffect star;
        SoundEffectInstance starInstance;

        public EndlessRunner()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            //Mouse cursor is visible
            IsMouseVisible = true;

            //Enables music to loop
            MediaPlayer.IsRepeating = true;

            //Sets the resolution of the game
            graphics.PreferredBackBufferWidth = 1000;
            graphics.PreferredBackBufferHeight = 600;
            graphics.ApplyChanges();

            //Stores the resolution of the game into variables
            screenWidth = graphics.GraphicsDevice.Viewport.Width;
            screenHeight = graphics.GraphicsDevice.Viewport.Height;

            //Intitalizes the star's colour
            starRandom = rng.Next(1, 101);
            StarRandomize();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            //Game Music
            inGameMusic = Content.Load<Song>("Resources/Sounds/Music/Jackie Chan - Police Story Theme OST (320  kbps)");
            mainMenuMusic = Content.Load<Song>("Resources/Sounds/Music/mainMenu");
            gameOverMusic = Content.Load<Song>("Resources/Sounds/Music/gameOver");

            //Initalizes main menu music
            MediaPlayer.Play(mainMenuMusic);

            //Sound Effects
            hkPunch = Content.Load<SoundEffect>("Resources/Sounds/Sound Effects/hkPunch");
            hkPunchInstance = hkPunch.CreateInstance();
            star = Content.Load<SoundEffect>("Resources/Sounds/Sound Effects/star");
            starInstance = star.CreateInstance();

            //Loads the Sprite Fonts
            titleFont = Content.Load<SpriteFont>("Resources/Texts/TitleFont");
            gameFont = Content.Load<SpriteFont>("Resources/Texts/GameItems");

            //Jackie Chan's Sprites
            jackieChan = Content.Load<Texture2D>("Resources/Images/Sprites/jcman");
            jackieChanRoll = Content.Load<Texture2D>("Resources/Images/Sprites/JackieChanRoll");
            jackieChanPosition = new Vector2(380, 423);
            jackieChanDuck = Content.Load<Texture2D>("Resources/Images/Sprites/DuckJackie");
            jackieChanDeath = Content.Load<Texture2D>("Resources/Images/Sprites/JackieDeath");

            //Sets the minimum height at which Jackie Chan walks
            minimumHeight = (int)jackieChanPosition.Y;

            //Loads the menu buttons and its location
            startButton = Content.Load<Texture2D>("Resources/Images/Background/startBtn");
            startBtnLoc = new Rectangle(355, 271, startButton.Width, startButton.Height);
            quitButton = Content.Load<Texture2D>("Resources/Images/Background/quitBtn");
            quitBtnLoc = new Rectangle(398, 447, (int)(quitButton.Width * 0.25), (int)(quitButton.Height * 0.25));
            controlsBtn = Content.Load<Texture2D>("Resources/Images/Background/controlsBtn");
            controlsBtnLoc = new Rectangle(396, 367, (int)(controlsBtn.Width * 0.25), (int)(controlsBtn.Height * 0.25));

            //Loads the control's information and its location
            controls = Content.Load<Texture2D>("Resources/Images/Background/controls");
            controlsLoc = new Rectangle(10, 10, (int)(controls.Width * 0.25), (int)(controls.Height * 0.25));
            collectableGuide = Content.Load<Texture2D>("Resources/Images/Background/collectables");
            collectableGuideLoc = new Rectangle(550, 10, (int)(collectableGuide.Width * 0.25), (int)(collectableGuide.Height * 0.25));
            escapeKey = Content.Load<Texture2D>("Resources/Images/Background/escapeKey");
            escapeKeyLoc = new Rectangle(30, 420, (int)(escapeKey.Width * 0.2), (int)(escapeKey.Height * 0.2));


            //Loads game over button options and its location
            retryBtn = Content.Load<Texture2D>("Resources/Images/Background/retryBtn");
            retryBtnLoc = new Rectangle(335, 300, (int)(retryBtn.Width * 0.5), (int)(retryBtn.Height * 0.5));
            menuBtn = Content.Load<Texture2D>("Resources/Images/Background/menuBtn");
            menuBtnLoc = new Rectangle(497, 280, (int)(menuBtn.Width * 0.5), (int)(menuBtn.Height * 0.5));

            //Loads background and its locations
            background = Content.Load<Texture2D>("Resources/Images/Background/background");
            bgLoc = new Rectangle(0, 0, screenWidth, screenHeight);
            bgLoc2 = new Rectangle(screenWidth, 0, screenWidth, screenHeight);

            //Loads bus Sprite and its location
            bus = Content.Load<Texture2D>("Resources/Images/Sprites/bussprite");
            busLoc = new Rectangle(105, 370, bus.Width, bus.Height);

            //Load Star Sprites and Collision Box
            redStar = Content.Load<Texture2D>("Resources/Images/Sprites/redStar");
            orangeStar = Content.Load<Texture2D>("Resources/Images/Sprites/orangeStar");
            greenStar = Content.Load<Texture2D>("Resources/Images/Sprites/greenStar");
            starCollide = new Rectangle(screenWidth + starOffset, 391, redStarFrameSize.X, redStarFrameSize.Y);

            //Loads score backdrop and its location
            itemBackDrop = Content.Load<Texture2D>("Resources/Images/Background/backDrop");
            itemBackDropLoc = new Rectangle(screenWidth - (int)(itemBackDrop.Width * 0.7), 0, (int)(itemBackDrop.Width * 0.7), (int)(itemBackDrop.Height * 0.7));

            //Loads box and its location
            box = Content.Load<Texture2D>("Resources/Images/Sprites/box");
            boxParameter = new Rectangle(screenWidth, 436, box.Width, box.Height);

            //Loads box crash sprite sheet
            boxCrash = Content.Load<Texture2D>("Resources/Images/Sprites/CrashCrate");

            //Loads grass obstacle and its location
            lowGrass = Content.Load<Texture2D>("Resources/Images/Sprites/platform");
            grassRectangle = new Rectangle(screenWidth, 0, (int)(lowGrass.Width * 0.15), (int)(lowGrass.Height * 0.39));

            //Loads gangster obstacle and its location
            gangster = Content.Load<Texture2D>("Resources/Images/Sprites/gangster");
            gangsterRec = new Rectangle(screenWidth, 360, (int)(gangster.Width * 0.40), (int)(gangster.Height * 0.40));

            //Loads gangster death picture
            gangsterDie = Content.Load<Texture2D>("Resources/Images/Sprites/GangsterDeath");

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            // TODO: Add your update logic here

            //Gets the state of keyboard and mouse
            kb = Keyboard.GetState();
            mouse = Mouse.GetState();

            //Changes game state based on gameplay
            switch (currentGameState)
            {
                case GameState.Start:
                    //Checks to see whether the player has hit the start button
                    if (MouseOnBtn(mouse, startBtnLoc))
                    {
                        if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton == ButtonState.Released)
                        {
                            MediaPlayer.Play(inGameMusic);
                            currentGameState = GameState.InGame;
                        }
                        prevMouse = mouse;
                    }
                    //Checks to see whether the player has hit the quit button
                    if (MouseOnBtn(mouse, quitBtnLoc))
                    {
                        if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton == ButtonState.Released)
                        {
                            Exit();
                        }
                        prevMouse = mouse;
                    }
                    if (MouseOnBtn(mouse, controlsBtnLoc))
                    {
                        if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton == ButtonState.Released)
                        {
                            currentGameState = GameState.Controls;
                        }
                    }
                    prevMouse = mouse;
                    break;
                case GameState.InGame:
                    //Increments current score every second
                    trackedTimeSurvived += gameTime.ElapsedGameTime.Milliseconds;
                    if (trackedTimeSurvived >= 1000 && !isHeDead)
                    {
                        trackedTimeSurvived = 0;
                        currentScore++;
                    }

                    //Increases the speed of game every 20 Seconds to a max of three times
                    speedTimeTrack += (isSpeedTracked * gameTime.ElapsedGameTime.Milliseconds);
                    if (speedTimeTrack >= TIME_BEFORE_SPEED_INCREASE)
                    {
                        //Resets the time track
                        speedTimeTrack = 0;

                        //Increments the number of times speed increases
                        counterSpeed++;

                        //Speeds game by increasing the framerate
                        TargetElapsedTime = new TimeSpan(0, 0, 0, 0, (int)(16.67 - counterSpeed * 2));
                    }

                    //Stops tracking and incrementing the speed value of the game
                    if (counterSpeed == MAXIMUM_SPEED_INCREASE)
                    {
                        isSpeedTracked = 0;
                    }

                    //Tracks the first few seconds before spawning an obstacle
                    timeTracked += (isTimeTracked * gameTime.ElapsedGameTime.Milliseconds);
                    if (timeTracked >= timeBeforeSpawn)
                    {
                        timeTracked = 0;
                        isTimeTracked = 0;
                        rollDie = rng.Next(1, 4);
                        isRandomized = true;
                    }

                    //The types of obstacles that will spawn and move
                    if (isRandomized)
                    {
                        if (rollDie == 1 || isObstaclesOn[2])
                        {
                            isObstaclesOn[2] = true;
                            gangsterRec.X -= speedGame;
                        }
                        if (rollDie == 2 || isObstaclesOn[0])
                        {
                            isObstaclesOn[0] = true;
                            boxParameter.X -= speedGame;
                            starCollide.X -= speedGame;
                        }
                        if (rollDie == 3 || isObstaclesOn[1])
                        {
                            isObstaclesOn[1] = true;
                            grassRectangle.X -= speedGame;
                        }
                    }

                    //Randomly changes the type of obstacle and increments score by three by players survives a obstacle
                    if (ChecksSpawn() == jackieChanPosition.X)
                    {
                        rollDie = rng.Next(1, 4);
                        currentScore += scoreItems[3];
                    }

                    //Positions the box to its inital position and resets type of star
                    if (boxParameter.X <= -boxParameter.X)
                    {
                        //Disables the crash animation
                        crashCurrentFrame = 0;
                        isCrash = false;

                        //Disables the spawning of the box
                        isObstaclesOn[0] = false;

                        //Places the position of the box and star to its initial
                        boxParameter.X = screenWidth;
                        starCollide.X = screenWidth + starOffset;

                        //Randomizes the next upcoming star
                        starRandom = rng.Next(1, 101);
                        StarRandomize();
                    }

                    //Positions the grass obstacle to its intial position
                    if (grassRectangle.X <= -grassRectangle.Width)
                    {
                        isObstaclesOn[1] = false;
                        grassRectangle.X = screenWidth;
                    }

                    //Positions the gangster to its intial position
                    if (gangsterRec.Y >= screenHeight)
                    {
                        isObstaclesOn[2] = false;
                        gangsterTrajectory.Y = INITIAL_VELOCITY;
                        isGangsterHit = false;
                        gangsterRec.X = screenWidth;
                        gangsterRec.Y = 360;
                    }

                    //Sets up the background
                    bgLoc.X -= speedGame;
                    bgLoc2.X -= speedGame;
                    if (bgLoc.X <= -screenWidth)
                    {
                        bgLoc.X = screenWidth;
                    }
                    if (bgLoc2.X <= -screenWidth)
                    {
                        bgLoc2.X = screenWidth;
                    }

                    //When Player Jumps
                    if (kb.IsKeyDown(Keys.Space) && !isHeDead)
                    {
                        //Sets collision box of player to roll
                        rollOffset = 15;
                        RollPlayer(gameTime);

                        //Gives the player vertical velocity when jumping
                        playerTrajectory += gravity;
                        jackieChanPosition -= playerTrajectory;

                        if (jackieChanPosition.Y >= minimumHeight)
                        {
                            //Sets collision box of player to normal
                            rollOffset = 0;

                            //Sets player back to ground level
                            jackieChanPosition.Y = minimumHeight;
                            playerTrajectory.Y = INITIAL_VELOCITY;
                        }
                    }
                    else if (kb.IsKeyUp(Keys.Space) && !isHeDead)
                    {
                        if (jackieChanPosition.Y < minimumHeight)
                        {
                            //Sets collision box of player to roll
                            rollOffset = 15;
                            RollPlayer(gameTime);

                            //Gives the player vertical velocity when jumping
                            playerTrajectory += gravity;
                            jackieChanPosition -= playerTrajectory;
                        }
                        if (jackieChanPosition.Y >= minimumHeight)
                        {
                            //Sets collision box of player to normal
                            rollOffset = 0;

                            //Sets player back to ground level
                            jackieChanPosition.Y = minimumHeight;
                            playerTrajectory.Y = INITIAL_VELOCITY;
                        }
                    }
                    //When the player Ducks Down
                    if (kb.IsKeyDown(Keys.Down) && !isHeDead)
                    {
                        isDuck = true;
                        DuckPlayer();
                    }
                    else if (kb.IsKeyUp(Keys.Down) && !isHeDead)
                    {
                        duckCurrentFrame = 0;
                        isDuck = false;
                    }

                    //Animates Jackie Chan walking
                    AnimatePlayer(gameTime);

                    //Stars Animation
                    StarAnimation(gameTime);

                    //Changes Jackie's collision box when ducking
                    if (isDuck)
                    {
                        jackieCollide = new Rectangle((int)jackieChanPosition.X,
                                        (int)jackieChanPosition.Y + duckCollision.Y, duckCollision.X, duckCollision.Y);
                    }
                    else if (!isDuck)
                    {
                        jackieCollide = new Rectangle((int)jackieChanPosition.X + rollOffset,
                                        (int)jackieChanPosition.Y + rollOffset, frameSize.X - (rollOffset * 2), frameSize.Y - (rollOffset * 2));
                    }

                    //Detects for Star Collision and increments the quantity
                    if (RecCollision(starCollide, jackieCollide))
                    {
                        //Checks to see which star is spawned and plays sound effect
                        if (isStarsVisible[0])
                        {
                            starInstance.Play();
                            isStarsVisible[0] = false;
                            starQuantity[0]++;
                        }
                        else if (isStarsVisible[1])
                        {
                            starInstance.Play();
                            isStarsVisible[1] = false;
                            starQuantity[1]++;
                        }
                        else if (isStarsVisible[2])
                        {
                            starInstance.Play();
                            isStarsVisible[2] = false;
                            starQuantity[2]++;
                        }
                    }
                    //Determines whether the player survives the gangster's obstacle
                    if (RecCollision(jackieCollide, gangsterRec) || isGangsterHit)
                    {
                        if (isDuck && jackieChanPosition.Y < minimumHeight || isGangsterHit)
                        {
                            //Plays the sound effect when player flying kicks the gangster
                            hkPunchInstance.Play();

                            //Enables the death state of the gangster
                            gangsterDeathRec = new Rectangle(gangsterRec.X, gangsterRec.Y, (int)(gangsterDie.Width * 0.65),
                                               (int)(gangsterDie.Height * 0.65));
                            isGangsterHit = true;

                            //Projectile motion of the gangster
                            gangsterTrajectory += gravity;
                            gangsterRec.Y -= (int)gangsterTrajectory.Y;
                        }
                        else
                        {
                            PlayerDies(gameTime);
                        }
                    }

                    //Box and Grass Collision with Jackie
                    if (RecCollision(jackieCollide, boxParameter) || RecCollision(grassRectangle, jackieCollide) || isHeDead)
                    {
                        PlayerDies(gameTime);
                    }
                    //Bus Collision with Box
                    if (RecCollision(busLoc, boxParameter))
                    {
                        isCrash = true;
                        CrashCrateAnimation(gameTime);
                    }
                    break;
                case GameState.GameOver:
                    //Resets the framerate to 60 frames
                    TargetElapsedTime = new TimeSpan(0, 0, 0, 0, (int)(16.67));

                    //Adds the bonus scores to the current score
                    currentScore += (starQuantity[0] * scoreItems[0]) + (starQuantity[1] * scoreItems[1]) + (starQuantity[2] * scoreItems[2]);
                    if (currentScore > highScore)
                    {
                        highScore = currentScore;
                    }

                    //Calls for the reset of the game's values to its initial
                    ResetValues();

                    //Takes the player back into the game
                    if (MouseOnBtn(mouse, retryBtnLoc))
                    {
                        if (mouse.LeftButton == ButtonState.Pressed)
                        {
                            ResetGame(inGameMusic);
                            currentGameState = GameState.InGame;
                        }
                    }

                    //Takes the player to the start of the game 
                    if (MouseOnBtn(mouse, menuBtnLoc))
                    {
                        if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton == ButtonState.Released)
                        {
                            ResetGame(mainMenuMusic);
                            currentGameState = GameState.Start;
                        }
                        prevMouse = mouse;
                    }
                    break;
                case GameState.Controls:
                    //Takes the user back to the main menu if they press Escape
                    if (kb.IsKeyDown(Keys.Escape) && prevKb.IsKeyUp(Keys.Escape))
                    {
                        currentGameState = GameState.Start;
                    }
                    prevKb = kb;
                    break;
            }
            base.Update(gameTime);
        }

        /// <summary>  
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();

            //Draws the background of the game
            spriteBatch.Draw(background, bgLoc, Color.White);
            spriteBatch.Draw(background, bgLoc2, Color.White);

            switch (currentGameState)
            {
                case GameState.Start:
                    //Draws the buttons in the main menu
                    spriteBatch.Draw(startButton, startBtnLoc, Color.White);
                    spriteBatch.Draw(quitButton, quitBtnLoc, Color.White);
                    spriteBatch.Draw(controlsBtn, controlsBtnLoc, Color.White);

                    //Draws the title of the game
                    spriteBatch.DrawString(titleFont, "Bus Chase", new Vector2(360, 140), Color.DarkRed);
                    break;
                case GameState.InGame:
                    //Draws the background
                    spriteBatch.Draw(itemBackDrop, itemBackDropLoc, Color.White * 0.2f);

                    //Draws the bus
                    spriteBatch.Draw(bus, busLoc, Color.White);

                    //Draws all of Jackie Chan's animations
                    if (isHeDead)
                    {
                        spriteBatch.Draw(jackieChanDeath, jackieChanPosition, deathJackie, Color.White);
                    }
                    else
                    {
                        //Draws all of Jackie's animations when he's alive
                        if (!isDuck)
                        {
                            if (jackieChanPosition.Y >= minimumHeight)
                            {
                                spriteBatch.Draw(jackieChan, jackieChanPosition, animJackie, Color.White);
                            }
                            else if (jackieChanPosition.Y < minimumHeight)
                            {
                                spriteBatch.Draw(jackieChanRoll, jackieChanPosition, rollJackie, Color.White);
                            }
                        }
                        else
                        {
                            spriteBatch.Draw(jackieChanDuck, jackieChanPosition, duckJackie, Color.White);
                        }
                    }

                    //Draws box obstacle and if it crashes into bus
                    if (isCrash)
                    {
                        spriteBatch.Draw(boxCrash, new Vector2(boxParameter.X, boxParameter.Y), boxCrashRec, Color.White);
                    }
                    else
                    {
                        spriteBatch.Draw(box, boxParameter, Color.White);
                    }

                    //Draws the grass Obstacle
                    spriteBatch.Draw(lowGrass, grassRectangle, Color.White);

                    //Draws Gangster's state whether he's alive
                    if (isGangsterHit)
                    {
                        spriteBatch.Draw(gangsterDie, gangsterDeathRec, Color.White);
                    }
                    else
                    {
                        spriteBatch.Draw(gangster, gangsterRec, Color.White);
                    }

                    //Draws the star collectables based on whether it spawned
                    if (isStarsVisible[1])
                    {
                        spriteBatch.Draw(redStar, new Vector2(starCollide.X, starCollide.Y), starAnim, Color.White);
                    }
                    else if (isStarsVisible[0])
                    {
                        spriteBatch.Draw(orangeStar, new Vector2(starCollide.X, starCollide.Y), starAnim, Color.White);
                    }
                    else if (isStarsVisible[2])
                    {
                        spriteBatch.Draw(greenStar, new Vector2(starCollide.X, starCollide.Y), starAnim, Color.White);
                    }

                    //Draws the current score to player to see
                    spriteBatch.DrawString(gameFont, "Score:" + currentScore, inGameScoreTxtLoc, Color.Violet);

                    //Draws the number of items collected
                    spriteBatch.DrawString(gameFont, "Stars Collected", starsTxtLoc, Color.Violet);
                    spriteBatch.DrawString(gameFont, "Common: " + starQuantity[0], orangeStarTxtLoc, Color.Orange);
                    spriteBatch.DrawString(gameFont, "Rare:" + starQuantity[1], redStarTxtLoc, Color.Red);
                    spriteBatch.DrawString(gameFont, "Legendary:" + starQuantity[2], greenStarTxtLoc, Color.LawnGreen);
                    break;
                case GameState.GameOver:
                    //Draws the Game Over title
                    spriteBatch.DrawString(titleFont, "Game Over", new Vector2(357, 160), Color.Tomato);

                    //Draws the backdrop for the scores
                    spriteBatch.Draw(itemBackDrop, itemBackDropLoc, Color.White * 0.2f);

                    //Draws the current and high score numbers
                    spriteBatch.DrawString(gameFont, "Current Score:" + currentScore, currentScoreLoc, Color.Violet);
                    spriteBatch.DrawString(gameFont, "Highscore:" + highScore, highScoreLoc, Color.Teal);

                    //Draws the button options
                    spriteBatch.Draw(retryBtn, retryBtnLoc, Color.White);
                    spriteBatch.Draw(menuBtn, menuBtnLoc, Color.White);
                    break;
                case GameState.Controls:
                    //Draws all of the information needed to succeed in the game
                    spriteBatch.Draw(controls, controlsLoc, Color.White);
                    spriteBatch.Draw(collectableGuide, collectableGuideLoc, Color.White);
                    spriteBatch.Draw(escapeKey, escapeKeyLoc, Color.White);
                    break;
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
        /// <summary>
        /// This animates the walking motion of the player
        /// </summary>
        /// <param name="gameTime">Snapshot of the game's timing state</param>
        private void AnimatePlayer(GameTime gameTime)
        {
            animJackie = new Rectangle(currentFrame * frameSize.X, 0, frameSize.X, frameSize.Y);
            timeSinceLastFrameJC += gameTime.ElapsedGameTime.Milliseconds;
            if (timeSinceLastFrameJC >= MILISECONDS_PER_FRAME)
            {
                timeSinceLastFrameJC = 0;
                currentFrame++;
                if (currentFrame >= PLAYER_SHEET_SIZE)
                {
                    currentFrame = 0;
                }
            }
        }
        /// <summary>
        /// This animates the rolling motion when user executes the jump action
        /// </summary>
        /// <param name="gameTime"></param>
        private void RollPlayer(GameTime gameTime)
        {
            rollJackie = new Rectangle(rollCurrentFrame * rollFrameSize.X, 0, rollFrameSize.X, rollFrameSize.Y);
            int rollMilisecondPerFrame = 100;
            rollTimeSinceLastFrame += gameTime.ElapsedGameTime.Milliseconds;
            if (rollTimeSinceLastFrame >= rollMilisecondPerFrame)
            {
                rollTimeSinceLastFrame = 0;
                rollCurrentFrame++;
                if (rollCurrentFrame >= ROLL_SHEET_SIZE)
                {
                    rollCurrentFrame = 0;

                }
            }
        }
        /// <summary>
        /// This executes the duck sliding motion when the player triggers the duck button
        /// </summary>
        /// <param name="gameTime"></param>
        private void DuckPlayer()
        {
            duckJackie = new Rectangle(duckCurrentFrame * duckFrameSize.X, 0, duckFrameSize.X, duckFrameSize.Y);
            duckCurrentFrame++;
            if (duckCurrentFrame == DUCK_SHEET_SIZE)
            {
                duckCurrentFrame = 2;
            }

        }
        /// <summary>
        /// Animates the death of Jackie Chan when collision is true
        /// </summary>
        /// <param name="gameTime"></param>
        private void DeathAnim()
        {
            deathJackie = new Rectangle(deathCurrentFrame * deathFrameSize.X, 0, deathFrameSize.X, deathFrameSize.Y);
            deathCurrentFrame++;
            if (deathCurrentFrame == DUCK_SHEET_SIZE)
            {
                deathCurrentFrame = 2;
            }
        }
        /// <summary>
        /// Animates the spinning motion of the Red Star Collectable
        /// </summary>
        /// <param name="gameTime"></param>
        private void StarAnimation(GameTime gameTime)
        {
            starAnim = new Rectangle(starCurrentFrame * redStarFrameSize.X, 0, redStarFrameSize.X, redStarFrameSize.Y);
            timeSinceLastFrameStar += gameTime.ElapsedGameTime.Milliseconds;
            if (timeSinceLastFrameStar >= MILISECONDS_PER_FRAME)
            {
                timeSinceLastFrameStar = 0;
                starCurrentFrame++;
                if (starCurrentFrame >= STAR_SHEET_SIZE)
                {
                    starCurrentFrame = 0;
                }
            }
        }
        /// <summary>
        /// Animates the Box Crash when Bus Crashes
        /// </summary>
        /// <param name="gameTime"></param>
        private void CrashCrateAnimation(GameTime gameTime)
        {
            boxCrashRec = new Rectangle(crashCurrentFrame * crashFrameSize.X, 0, crashFrameSize.X, crashFrameSize.Y);
            timeSinceLastFrameCrash += gameTime.ElapsedGameTime.Milliseconds;
            if (timeSinceLastFrameCrash >= MILISECONDS_PER_FRAME)
            {
                crashCurrentFrame++;
                if (crashCurrentFrame >= CRASH_SHEET_SIZE)
                {
                    crashCurrentFrame = 6;
                }
            }
        }
        /// <summary>
        /// Detects to see whether their is a collision between two rectangles
        /// </summary>
        /// <param name="r1"></param>
        /// <param name="r2"></param>
        /// <returns></returns>
        private bool RecCollision(Rectangle r1, Rectangle r2)
        {
            if (r1.Bottom < r2.Top || r1.Top > r2.Bottom || r1.Right < r2.Left || r1.Left > r2.Right)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        /// <summary>
        /// Randomizes which stars is to be spawned based on its rarity
        /// </summary>
        private void StarRandomize()
        {
            //Stores the chances of stars spawning
            const int greenStarChance = 1;
            const int redStarChance = 15;
            const int orangeStarChance = 84;

            //Determines which star to spawn based on chance
            if (starRandom <= greenStarChance && !isStarsVisible[1] && !isStarsVisible[0])
            {
                isStarsVisible[2] = true;
            }
            else if (starRandom <= redStarChance && !isStarsVisible[0] && !isStarsVisible[2])
            {
                isStarsVisible[1] = true;
            }
            else if (starRandom <= orangeStarChance && !isStarsVisible[1] && !isStarsVisible[2])
            {
                isStarsVisible[0] = true;
            }
        }
        /// <summary>
        /// Starts the animation and projectile motion when Jackie dies
        /// </summary>
        private void PlayerDies(GameTime gameTime)
        {
            //Moves the bus when player is dead
            busLoc.X += SPEED_BUS;

            //Stops the movement of the game
            speedGame = 0;

            //Sets the player state to dead and animates death
            isHeDead = true;
            DeathAnim();

            //Projectie motion of the player when dead
            deathProjectile += gravity;
            jackieChanPosition -= deathProjectile;

            //Changes gamestate to game over when player falls off screen
            if (jackieChanPosition.Y >= screenHeight)
            {
                //Plays the game over music
                MediaPlayer.Play(gameOverMusic);
                currentGameState = GameState.GameOver;
            }
        }
        /// <summary>
        /// Checks to see which obstacle is next for the player to maneuver
        /// </summary>
        /// <returns></returns>
        private int ChecksSpawn()
        {
            if (isObstaclesOn[1])
            {
                return grassRectangle.X;
            }
            else if (isObstaclesOn[2])
            {
                return gangsterRec.X;
            }
            else if (isObstaclesOn[0])
            {
                return boxParameter.X;
            }
            else
            {
                return 0;
            }
        }
        /// <summary>
        /// Checks to see if the mouse cursor is within a button's area
        /// </summary>
        /// <param name="mouse"></param>
        /// <param name="r1"></param>
        /// <returns></returns>
        private bool MouseOnBtn(MouseState mouse, Rectangle r1)
        {
            if (r1.X <= mouse.X && mouse.X <= r1.X + r1.Width && r1.Y <= mouse.Y && r1.Y + r1.Height >= mouse.Y)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Resets the game's values back to its initial values
        /// </summary>
        private void ResetValues()
        {
            //Sets the speed back to the intial speed
            speedGame = 10;

            //Resets the crash frame
            crashCurrentFrame = 0;

            //Sets Player's position back to ground level
            jackieChanPosition = new Vector2(380, minimumHeight);
            isHeDead = false;

            //Resets the tracked time and random spawn
            speedTimeTrack = 0;
            counterSpeed = 0;
            timeTracked = 0;
            trackedTimeSurvived = 0;
            rollDie = 0;

            //Resets the bus back to it's inital location
            busLoc = new Rectangle(105, 370, bus.Width, bus.Height);

            //Resets whether the obstacles were on screen
            isRandomized = false;
            isObstaclesOn = new bool[] { false, false, false };
            isCrash = false;


            //Resets the obstacles location to the inital location
            starCollide = new Rectangle(screenWidth + starOffset, 391, redStarFrameSize.X, redStarFrameSize.Y);
            boxParameter = new Rectangle(screenWidth, 436, box.Width, box.Height);
            grassRectangle = new Rectangle(screenWidth, 0, (int)(lowGrass.Width * 0.15), (int)(lowGrass.Height * 0.39));
            gangsterRec = new Rectangle(screenWidth, 360, (int)(gangster.Width * 0.40), (int)(gangster.Height * 0.40));

            //Randomizes the star that is spawned
            starRandom = rng.Next(1, 101);
            StarRandomize();
            isStarsVisible = new bool[] { false, false, false };
            starQuantity = new int[] { 0, 0, 0 };

            //Resets all the object's tracjectory speed
            playerTrajectory.Y = INITIAL_VELOCITY;
            deathProjectile.Y = INITIAL_VELOCITY;
            gangsterTrajectory.Y = INITIAL_VELOCITY;
        }
        /// <summary>
        /// Resets the values that affects the gameplay
        /// </summary>
        /// <param name="music"></param>
        private void ResetGame(Song music)
        {
            //Randomizes the next spawn obstacle
            rollDie = rng.Next(1, 4);

            //Resets the current score back to zero
            currentScore = 0;

            //Enables the tracking of time in gameplay
            isTimeTracked = 1;
            isSpeedTracked = 1;

            //Plays selected music
            MediaPlayer.Play(music);
        }
    }
}