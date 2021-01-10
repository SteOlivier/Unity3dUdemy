using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Timers;
using UnityEngine;

public class Hacker : MonoBehaviour
{
    // Game State
    private int level;
    private int guesAttempts;
    private int passwordNumber;
    private AudioSource audioHumm;
    private UnityEngine.UI.Text baseText;
    private FontManager loadedFonts;
    private Display mainDisplay;

    enum Screen {  MainMenu, PasswordWait, WinScreen};
    enum Pitch { Lower, Neutral, Higher};

    private float pitchChange = 0.001f;
    private float stereoPanChange = 0.0001f;
    private float randomTimeLength;
    private float randomTimeShuffle = 1f;
    string[][] passwords = new string[3][];

    Screen currentScreen;
    Pitch pitchDirection;


    /// <summary>
    /// Level 2 shuffle timer
    /// </summary>
    //private Timer timeBusy;
    private float lastExecutedTime;
    private int currentInterval;
    //private Stopwatch warningTime;
    private Color setColor;
    private Color resetColor;
    private List<int> correctIndexes;

    // Start is called before the first frame update
    void Start()
    {

        randomTimeLength = UnityEngine.Random.Range(0.02f, 0.8f);
        //timeBusy = new Timer();

        //warningTime = new Stopwatch();
        resetColor = new Color(0, 1, 0.279f, 1);
        setColor = resetColor;
        correctIndexes = new List<int>(100);

        ShowMainMenu();
        string[] easyPasswords = { "principle", "teacher", "a+ student", "textbooks", "classroom", "students", "assembly", "curriculum" };
        string[] mediPasswords = { "waterboarding", "infiltrate", "suitcases", "cloak and dagger", "central intelligence", "cyanide pills", "neutralize", "terminate", "microfilm", "assassinate", "interrogation", "stabilize", "coupe", "brainwashing LSD", "dossier" };
        string[] hardPasswords = { "I am become Death, the destroyer of worlds", "Pew pew pew pew", "ET phone home", "Technology isn't always the best way to communicate", "Unity for the wrong reason can be dangerous", "Take us to your leader", "We come in peace human" };
        passwords[0] = easyPasswords;
        passwords[1] = mediPasswords;
        passwords[2] = hardPasswords;
        NotifyCommandHandlers();
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateAudioLoop();
        UpdateFontsAlien();
        //UpdateCountdownTimer();
        if (Time.time*1000 > (1000*lastExecutedTime)+currentInterval) UpdateCountdownTimer("this executed", null);
        UpdateTextColor();
    }

    private void UpdateTextColor()
    {
        if (baseText.color != setColor) baseText.color = setColor;
    }

    private void UpdateCountdownTimer(object sender, ElapsedEventArgs e)
    {
        //timeBusy.Enabled = false;
        if (level == 2)
        {
            //if (timeBusy.Interval > 1)
            if (currentInterval > 1)
            {
                //timeBusy.Interval = timeBusy.Interval * 0.985;
                currentInterval = (int)(currentInterval * 0.985);
            }
            else
            {
                currentInterval += 123;
                if (5 - guesAttempts <= 0)
                {
                    GuessPassword("You died!!!");
                    return;
                }
                guesAttempts++;
            }
            Terminal.ClearScreen();
            Terminal.WriteLine("Time to reverse trace: [");
            var timeLeft = 0;
            for (int ii = 0; ii < (int)(currentInterval / 50); ii++)
            {
                Terminal.Write("=");
                timeLeft++;
            }
            for (int ii = (int)(currentInterval / 50); ii < 10; ii++)
            {
                Terminal.Write(" ");
            }
            Terminal.Write("]");
            Terminal.WriteLine($"Hint for the password: {passwords[level - 1][passwordNumber].Anagram(KeepIndexes: correctIndexes)} ({5 - guesAttempts})"); // Todo: create a worded anagram class

            if (timeLeft < 2)
            {

                if (Time.time % 2000 > 999)
                {
                    try
                    {
                        setColor = new UnityEngine.Color(1, 0.094f, 0, 1);
                    }
                    catch (Exception ex)
                    {
                        print(ex.Message);
                    }
                }
                else
                {
                    //baseText.color = new UnityEngine.Color(0, 255, 76, 255);
                    try
                    {
                        setColor = new UnityEngine.Color(0, 1, 0.297f, 1);
                    }
                    catch (Exception ex)
                    {
                        print(ex.Message);
                    }
                }
            }
        }

        lastExecutedTime = Time.time;
    }


    private void UpdateFontsAlien()
    {
        if (level != 3)
        {
            baseText.font = loadedFonts.get.GetFont(loadedFonts.get.fonts[0].name);
        }
        else if (Time.time % randomTimeShuffle <= 0.01)
        {
            randomTimeShuffle = UnityEngine.Random.Range(0.5f, 5f);
            baseText.font = loadedFonts.get.GetFont(loadedFonts.get.fonts[UnityEngine.Random.Range(1, loadedFonts.get.fonts.Length)].name);
        }
    }

    private void UpdateAudioLoop()
    {
        // reset audio pitch if not level 3
        if (level != 3)
        {
            audioHumm.pitch = 1;
            audioHumm.panStereo = 0;
            audioHumm.spatialBlend = 0;
            return;
        }
        if (audioHumm.pitch > 3f)
        {
            pitchDirection = Pitch.Lower;
            pitchChange = -Math.Abs(pitchChange);
        }
        if (audioHumm.pitch < -3f)
        {
            pitchDirection = Pitch.Higher;
            pitchChange = Math.Abs(pitchChange);
        }
        if (audioHumm.panStereo < -0.5f)
        {
            stereoPanChange = Math.Abs(stereoPanChange);
        }
        else if (audioHumm.panStereo > 0.5f)
        {
            stereoPanChange = -Math.Abs(stereoPanChange);
        }

        audioHumm.pitch += pitchChange;
        audioHumm.panStereo += stereoPanChange;
        //randomTimeLength =  // only once
        if (Time.time % (2 * randomTimeLength) >= randomTimeLength)
        {
            if (audioHumm.pitch > 2.8f) pitchDirection = Pitch.Lower;
            else if (audioHumm.pitch < -2.8f) pitchDirection = Pitch.Higher;
            else pitchDirection = Pitch.Neutral;


            switch (pitchDirection)
            {
                case Pitch.Higher:
                    pitchChange = UnityEngine.Random.Range(0.001f, 0.05f);
                    break;
                case Pitch.Lower:
                    pitchChange = UnityEngine.Random.Range(-0.05f, -0.001f);
                    break;
                case Pitch.Neutral:
                    pitchChange = UnityEngine.Random.Range(-0.05f, 0.05f);
                    break;
                default:
                    break;
            }

            audioHumm.spatialBlend = UnityEngine.Random.Range(0, 2);
            stereoPanChange = UnityEngine.Random.Range(-0.01f, 0.01f);

            randomTimeLength = UnityEngine.Random.Range(0.02f, 0.5f); // reset time till change
        }
    }


    void OnUserInput(string input)
    {
        print("The user typed " + input);
        if (input.Equals("menu", StringComparison.InvariantCultureIgnoreCase)) ShowMainMenu();
        else if (Screen.MainMenu == currentScreen) SelectLevel(input);
        else if (Screen.PasswordWait == currentScreen) GuessPassword(input);//
        //else Terminal.WriteLine("   .");

        
    }

    private void GuessPassword(string input)
    {
        //Terminal.WriteLine($"Hint for the password: {passwords[level - 1][1].Anagram()}");
        correctIndexes.Clear();
        var lowerInput = input.ToLower();
        var lowerPassword = passwords[level - 1][passwordNumber].ToLower();
        if (lowerInput == lowerPassword)
        {

            if (level == 3 && passwordNumber == 0)
            {
                if (mainDisplay != null) mainDisplay.setScreenWidth(80, 33);
                Terminal.ClearScreen();
                baseText.font = loadedFonts.fonts[0];
                baseText.fontSize = 20;
                baseText.horizontalOverflow = HorizontalWrapMode.Overflow;
                Terminal.WriteLine(@"                        .-1+ooooosoooo+1:.");
                Terminal.WriteLine(@"                  `:oyhyooo-   -:ssds+oy#NN#ho:`");
                Terminal.WriteLine(@"               :oso:-     :o- :#No`o#############s:");
                Terminal.WriteLine(@"            -sh+`       -:+NsN#####################Ny:");
                Terminal.WriteLine(@"          1y+.          :11#hyy:.#####h.#NhNNoddN######+`");
                Terminal.WriteLine(@"        1h1            1##s:`   `:::hy1s##h####y1N######N+");
                Terminal.WriteLine(@"      .h+            -:N#######1::::::::::+#####N##########-");
                Terminal.WriteLine(@"     1h.           1o#####################od#####NyN########o");
                Terminal.WriteLine(@"    oy`           1#######################yy1y####y##oood####s");
                Terminal.WriteLine(@"   +h           -+##########################..#######y``.1##Nhs:");
                Terminal.WriteLine(@"  -#`           d###########################NN:-NN##d-````1##-#: ");
                Terminal.WriteLine(@"  h1           .#N##############################1---```````.d.:# ");
                Terminal.WriteLine(@" .#:             oh#####Nhhhd#####################Nd````````oh`d: ");
                Terminal.WriteLine(@" 1##.              yyyyy1   -yh#################Ndyo```````````yo ");
                Terminal.WriteLine(@" o##y1                        -###############Nds-.````````````oy ");
                Terminal.WriteLine(@" +####o+`                     -################:- `````````````ss ");
                Terminal.WriteLine(@" .######1                     `+s#############y ```````````````d:");
                Terminal.WriteLine(@"  d####d.                       .#############o ```.``````````:#");
                Terminal.WriteLine(@"  -#####.                     `yh#############:`hdd-`````````.d1 ");
                Terminal.WriteLine(@"   o###Ns                      .:###########:. +##d``.```````sy ");
                Terminal.WriteLine(@"    s##d`                       .##########h   hh```````````oy  ");
                Terminal.WriteLine(@"     +##o                       `hd#####ds    ````````````.ys   ");
                Terminal.WriteLine(@"      -#N.                        +#s++1.`   ````````````1d:    ");
                Terminal.WriteLine(@"        +d:                       `-       ````````````:yo`     ");
                Terminal.WriteLine(@"         `+y1                              ``````````1yo`       ");
                Terminal.WriteLine(@"            :ss:                         `````````:sy1          ");
                Terminal.WriteLine(@"              `1ss+-                   ```````-+ss1`            ");
                Terminal.WriteLine(@"                  .1ooo+:-` .:::::::111+ysosso1.                ");
                Terminal.WriteLine(@"                       `-:+osyhhhhhhyso+1-`                     ");
                level = 0;
                currentScreen = Screen.MainMenu; // ehhh
            }
            else if (level == 3 && passwordNumber == 1)
            {
                if (mainDisplay != null) mainDisplay.setScreenWidth(80, 33);
                Terminal.ClearScreen();
                baseText.fontSize = 20;
                baseText.horizontalOverflow = HorizontalWrapMode.Overflow;
                Terminal.WriteLine(@"                                               ____________");
                Terminal.WriteLine(@"                                --)-----------|____________|");
                Terminal.WriteLine(@"                                              ,'       ,'");
                Terminal.WriteLine(@"                -)------========            ,'  ____ ,'");
                Terminal.WriteLine(@"                         `.    `.         ,'  ,'__ ,'                   ");
                Terminal.WriteLine(@"                           `.    `.     ,'       ,'                     ");
                Terminal.WriteLine(@"                             `.    `._,'_______,'__                     ");
                Terminal.WriteLine(@"                               [._ _| ^--      || |                     ");
                Terminal.WriteLine(@"                       ____,...-----|__________ll_|\                    ");
                Terminal.WriteLine(@"      ,.,..-------""""""""""     ""----'                 ||                ");
                Terminal.WriteLine(@"  .- """" |=========================== ______________ |                    ");
                Terminal.WriteLine(@"   ""-...l_______________________    |  |'      || |_]                     ");
                Terminal.WriteLine(@"                                [`-.| __________ll_ |                      ");
                Terminal.WriteLine(@"                              , '    ,' `.        `.                       ");
                Terminal.WriteLine(@"                            , '    ,'     `.    ____`.                  ");
                Terminal.WriteLine(@"                -)-------- -========        `.  `.____`.                ");
                Terminal.WriteLine(@"                                             `.        `.               ");
                Terminal.WriteLine(@"    Incom's T-65B X-wing                       `.________`.             ");
                Terminal.WriteLine(@"                            --)------------ -| ___________ |");
                level = 0;
                currentScreen = Screen.MainMenu; // ehhh
            }
            else if (level == 3 && passwordNumber == 3)
            {
                if (mainDisplay != null) mainDisplay.setScreenWidth(80, 33);
                Terminal.ClearScreen();
                baseText.fontSize = 20;
                baseText.horizontalOverflow = HorizontalWrapMode.Overflow;
                Terminal.WriteLine(@"                      .,,uod8B8bou,,.                              ");
                Terminal.WriteLine(@"              ..,uod8BBBBBBBBBBBBBBBBRPFT?l!i:.                    ");
                Terminal.WriteLine(@"         ,=m8BBBBBBBBBBBBBBBRPFT?!||||||||||||||                   ");
                Terminal.WriteLine(@"         !...:!TVBBBRPFT||||||||||!!^^""'   ||||                   ");
                Terminal.WriteLine(@"         !.......:!?|||||!!^^""'            ||||                   ");
                Terminal.WriteLine(@"         !.........||||                     ||||                   ");
                Terminal.WriteLine(@"         !.........||||  ##                 ||||                   ");
                Terminal.WriteLine(@"         !.........||||                     ||||                   ");
                Terminal.WriteLine(@"         !.........||||                     ||||                   ");
                Terminal.WriteLine(@"         !.........||||                     ||||                   ");
                Terminal.WriteLine(@"         !.........||||                     ||||                   ");
                Terminal.WriteLine(@"         `.........||||                    ,||||                   ");
                Terminal.WriteLine(@"          .;.......||||               _.-!!|||||                   ");
                Terminal.WriteLine(@"   .,uodWBBBBb.....||||       _.-!!|||||||||!:'                    ");
                Terminal.WriteLine(@"!YBBBBBBBBBBBBBBb..!|||:..-!!|||||||!iof68BBBBBb....               ");
                Terminal.WriteLine(@"!..YBBBBBBBBBBBBBBb!!||||||||!iof68BBBBBBRPFT?!::   `.             ");
                Terminal.WriteLine(@"!....YBBBBBBBBBBBBBBbaaitf68BBBBBBRPFT?!:::::::::     `.           ");
                Terminal.WriteLine(@"!......YBBBBBBBBBBBBBBBBBBBRPFT?!::::::;:!^""`;:::       `.        ");
                Terminal.WriteLine(@"!........YBBBBBBBBBBRPFT?!::::::::::^''...::::::;         iBBbo.   ");
                Terminal.WriteLine(@"`..........YBRPFT?!::::::::::::::::::::::::;iof68bo.      WBBBBbo. ");
                Terminal.WriteLine(@"  `..........:::::::::::::::::::::::;iof688888888888b.     `YBBBP^'");
                Terminal.WriteLine(@"    `........::::::::::::::::;iof688888888888888888888b.     `     ");
                Terminal.WriteLine(@"      `......:::::::::;iof688888888888888888888888888888b.         ");
                Terminal.WriteLine(@"        `....:::;iof688888888888888888888888888888888899fT!        ");
                Terminal.WriteLine(@"          `..::!8888888888888888888888888888888899fT|!^""'         ");
                Terminal.WriteLine(@"            `' !!988888888888888888888888899fT|!^""'               ");
                Terminal.WriteLine(@"                `!!8888888888888888899fT|!^""'                     ");
                Terminal.WriteLine(@"                  `!988888888899fT|!^""'                           ");
                Terminal.WriteLine(@"                    `!9899fT|!^""'                                 ");
                Terminal.WriteLine(@"                      `!^""'                                       ");
                level = 0;
                currentScreen = Screen.MainMenu; // ehhh
            }
            else if (level == 3 && passwordNumber == 4)
            {
                if (mainDisplay != null) mainDisplay.setScreenWidth(80, 33);
                Terminal.ClearScreen();
                baseText.fontSize = 28;
                baseText.horizontalOverflow = HorizontalWrapMode.Overflow;
                Terminal.WriteLine(@"           .-""""""""-.       .-""""""""-.              ");
                Terminal.WriteLine(@"          /        \     /        \                     ");
                Terminal.WriteLine(@"         /_        _\   /_        _\                    ");
                Terminal.WriteLine(@"        // \      / \\ // \      / \\                   ");
                Terminal.WriteLine(@"        |\__\    /__/| |\__\    /__/|                   ");
                Terminal.WriteLine(@"         \    ||    /   \    ||    /                    ");
                Terminal.WriteLine(@"          \        /     \        /                     ");
                Terminal.WriteLine(@"           \  __  /       \  __  /                      ");
                Terminal.WriteLine(@"   .-""""""""-. '.__.'.-""""""""-. '.__.'.-""""""""-.   ");
                Terminal.WriteLine(@"  /        \ |  |/        \ |  |/        \              ");
                Terminal.WriteLine(@" /_        _\|  /_        _\|  /_        _\             ");
                Terminal.WriteLine(@"// \      / \\ // \      / \\ // \      / \\            ");
                Terminal.WriteLine(@"|\__\    /__/| |\__\    /__/| |\__\    /__/|            ");
                Terminal.WriteLine(@" \    ||    /   \    ||    /   \    ||    /             ");
                Terminal.WriteLine(@"  \        /     \        /     \        /              ");
                Terminal.WriteLine(@"   \  __  /       \  __  /       \  __  /               ");
                Terminal.WriteLine(@"    '.__.'         '.__.'         '.__.'                ");
                Terminal.WriteLine(@"jgs  |  |           |  |           |  |                 ");
                Terminal.WriteLine(@"     |  |           |  |           |  |                 ");
                level = 0;
                currentScreen = Screen.MainMenu; // ehhh
            }
            else if (level == 3 && passwordNumber == 6) //peace
            {
                if (mainDisplay != null) mainDisplay.setScreenWidth(65, 33);
                Terminal.ClearScreen();
                baseText.fontSize = 24;
                baseText.horizontalOverflow = HorizontalWrapMode.Overflow;
                Terminal.WriteLine(@"       _..._               ");
                Terminal.WriteLine(@"     .'     '.             ");
                Terminal.WriteLine(@"    /`\     /`\            ");
                Terminal.WriteLine(@"   (__|     |__)|\     /| ");
                Terminal.WriteLine(@"   (     ""     ) \\   // ");
                Terminal.WriteLine(@"    \         /   \\_//   ");
                Terminal.WriteLine(@"     \   _   /  |\|` /    ");
                Terminal.WriteLine(@"      '.___.'   \____/     ");
                Terminal.WriteLine(@"       (___)    (___)      ");
                Terminal.WriteLine(@"     /`     `\  / /        ");
                Terminal.WriteLine(@"    |         \/ /         ");
                Terminal.WriteLine(@"    | |     |\  /          ");
                Terminal.WriteLine(@"    | |     | ""`          ");
                Terminal.WriteLine(@"    | |     |              ");
                Terminal.WriteLine(@"    | |     |              ");
                Terminal.WriteLine(@"    |_|_____|              ");
                Terminal.WriteLine(@"   (___)_____)             ");
                Terminal.WriteLine(@"   /    \   |              ");
                Terminal.WriteLine(@"  /   |\|   |              ");
                Terminal.WriteLine(@" //||\\  Y  |              ");
                Terminal.WriteLine(@"|| || \\ |  |              ");
                Terminal.WriteLine(@"|/ \\ |\||  |              ");
                Terminal.WriteLine(@"    \||__|__|              ");
                Terminal.WriteLine(@"     (___|___)             ");
                Terminal.WriteLine(@"     /   A   \             ");
                Terminal.WriteLine(@"    /   / \   \            ");
                Terminal.WriteLine(@"   \___/   \___/           ");
                level = 0;
                currentScreen = Screen.MainMenu; // ehhh

            }
            else if (level == 3 && passwordNumber == 5) // leader
            {
                if (mainDisplay != null) mainDisplay.setScreenWidth(80, 33);
                Terminal.ClearScreen();
                baseText.fontSize = 20;
                baseText.horizontalOverflow = HorizontalWrapMode.Overflow;
                Terminal.WriteLine(@".     .       .  .   . .   .   . .    +  .                  ");
                Terminal.WriteLine(@"  .     .  :     .    .. :. .___---------___.               ");
                Terminal.WriteLine(@"       .  .   .    .  :.:. _"".^ .^ ^.  '.. :""-_. .        ");
                Terminal.WriteLine(@"    .  :       .  .  .:../:            . .^  :.:\.          ");
                Terminal.WriteLine(@"        .   . :: +. :.:/: .   .    .        . . .:\         ");
                Terminal.WriteLine(@" .  :    .     . _ :::/:               .  ^ .  . .:\        ");
                Terminal.WriteLine(@"  .. . .   . - : :.:./.                        .  .:\       ");
                Terminal.WriteLine(@"  .      .     . :..|:                    .  .  ^. .:|      ");
                Terminal.WriteLine(@"    .       . : : ..||        .                . . !:|      ");
                Terminal.WriteLine(@"  .     . . . ::. ::\(                           . :)/      ");
                Terminal.WriteLine(@" .   .     : . : .:.|. ######              .#######::|      ");
                Terminal.WriteLine(@"  :.. .  :-  : .:  ::|.#######           ..########:|       ");
                Terminal.WriteLine(@" .  .  .  ..  .  .. :\ ########          :######## :/       ");
                Terminal.WriteLine(@"  .        .+ :: : -.:\ ########       . ########.:/        ");
                Terminal.WriteLine(@"    .  .+   . . . . :.:\. #######       #######..:/         ");
                Terminal.WriteLine(@"      :: . . . . ::.:..:.\           .   .   ..:/           ");
                Terminal.WriteLine(@"   .   .   .  .. :  -::::.\.       | |     . .:/            ");
                Terminal.WriteLine(@"      .  :  .  .  .-:."":.::.\             ..:/             ");
                Terminal.WriteLine(@" .      -.   . . . .: .:::.:.\.           .:/               ");
                Terminal.WriteLine(@".   .   .  :      : ....::_:..:\   ___.  :/                 ");
                Terminal.WriteLine(@"   .   .  .   .:. .. .  .: :.:.:\       :/                  ");
                Terminal.WriteLine(@"     +   .   .   : . ::. :.:. .:.|\  .:/|                   ");
                Terminal.WriteLine(@"     .         +   .  .  ...:: ..|  --.:|                   ");
                Terminal.WriteLine(@".      . . .   .  .  . ... :..:..""(  ..)""                 ");
                Terminal.WriteLine(@" .   .       .      :  .   .: ::/  .  .::\                  ");
                level = 0;
                currentScreen = Screen.MainMenu; // ehhh

            }
            else
            {
                // Todo: victory condition
                Terminal.ClearScreen();
                Terminal.WriteLine("Victory");
                setColor = resetColor;
                level = 0;
                currentScreen = Screen.MainMenu; // ehhh
            }

            
        }
        else if (5-guesAttempts>0) //todo: fail condition
        {
            for (int ii = 0; ii < lowerPassword.Length && ii < lowerInput.Length; ii++)
            {
                if (lowerInput[ii] == lowerPassword[ii])
                {
                    correctIndexes.Add(ii);
                    print($"correct Index = {ii}");
                }
            }
            StartGame(5-guesAttempts++);
            //try again?
        }
        else
        {
            Terminal.ClearScreen();
            Terminal.WriteLine("Defeat");
            setColor = new UnityEngine.Color(1, 0.094f, 0, 1); ;
            
            level = 0;
            currentScreen = Screen.MainMenu; // ehhh
        }
        return;
        //throw new NotImplementedException();
    }

    private void SelectLevel(string input)
    {
        int iLevel = 0;
        baseText.fontSize = 39;
        setColor = resetColor;
        baseText.horizontalOverflow = HorizontalWrapMode.Wrap;
        if (mainDisplay != null) mainDisplay.setScreenWidth(40, 20);

        correctIndexes = new List<int>();
        if (int.TryParse(input, out iLevel))
        {
            if (iLevel > 3 || iLevel<1)
            {
                Terminal.WriteLine("Invalid level chosen");
                return;
            }
            guesAttempts = 0;
            currentScreen = Screen.PasswordWait;
            level = iLevel; // set level 
            for (int ii = 0; ii < baseText.font.fontNames.Length; ii++)
            {
                print(baseText.font.fontNames[ii]);
            }
            print(baseText.font.name);
            print(baseText.font.name);
            if (level == 3) baseText.font = loadedFonts.get.GetFont("symbolzm");

            passwordNumber = UnityEngine.Random.Range(0, passwords[level-1].Length); // set password
            StartGame(5-guesAttempts++);
        }
        else
        {
            //Terminal.WriteLine("Invalid input");
            ShowMainMenu("Invalid input");
            setColor = resetColor;
        }
    }

    void StartGame(int GuessesRemaining)
    {
        
        switch (level)
        {
            case 1:
                Terminal.ClearScreen();
                setColor = resetColor;
                Terminal.WriteLine($"Hint for the password: {passwords[level - 1][passwordNumber].Anagram(KeepIndexes:correctIndexes)} ({GuessesRemaining})"); // Todo: make a visual representation for the words
                break;
            case 2:
                Terminal.ClearScreen();
                setColor = resetColor;
                if (guesAttempts == 1)
                {
                    currentInterval = 527;
                    //timeBusy.Interval = 527;
                    //timeBusy.Elapsed -= UpdateCountdownTimer;
                    //timeBusy.Elapsed += UpdateCountdownTimer;
                    UpdateCountdownTimer("doesn't matter", null);
                    //warningTime = Stopwatch.StartNew();
                    //timeBusy.Enabled = true;
                }
                //Terminal.WriteLine($"Hint for the password: {passwords[level - 1][passwordNumber].Anagram()} ({GuessesRemaining})"); // Todo: create a worded anagram class
                break;
            case 3:
                Terminal.ClearScreen();
                Terminal.WriteLine($"{passwords[level - 1][passwordNumber]}                          -({GuessesRemaining})");
                break;
            default:
                throw new NotImplementedException("This level has not been created yet.");
                
                break;
        }
        
        //if (GuessCount > 0) Terminal.WriteLine("Incorrect Password"); 
        //Terminal.ChangePitch
        //if (level != 3) 
        //else 
        //Terminal.WriteLine("You have chosen level " + level);

    }

    void ShowMainMenu(string priorWarning = null)
    {
        Terminal.ClearScreen();
        level = 0;
        currentScreen = Screen.MainMenu;
        if (!string.IsNullOrEmpty(priorWarning)) Terminal.WriteLine(priorWarning);
        Terminal.WriteLine($"What would you like to hack into?\n\nPress 1 for Grade 12 IT office");
        Terminal.WriteLine("Press 2 to try hack FBI, CIA or KGB");
        Terminal.WriteLine("Press 3 to defend earth against an\n Alien virus attack\n\n");
        Terminal.WriteLine("Enter your selection:");
    }

    void NotifyCommandHandlers()
    {
        
        var allGameObjects = FindObjectsOfType<MonoBehaviour>();
        foreach (MonoBehaviour mb in allGameObjects)
        {
            //print(mb);
            //Terminal.WriteLine(mb.GetType().ToString());
            if (mb.GetType() == typeof(UnityEngine.UI.Text))
            {
                UnityEngine.UI.Text TerminalText = (UnityEngine.UI.Text)mb;
                //var theColor = TerminalText.GetComponent<Color>();
                var theFontM = TerminalText.GetComponent<FontManager>();
                loadedFonts = theFontM;
                //print(theColor);
                //print(theFontM);
                //print(theFontM.fonts.Length);
                //TerminalText.fontSize = 26;
                print(TerminalText.fontSize);
                baseText = TerminalText;
            }
            else if (mb.GetType() == typeof(Terminal))
            {
                Terminal baseTerminal = (Terminal)mb;
                var audioS = baseTerminal.GetComponent<AudioSource>();
                audioHumm = audioS;
                //audioS.pitch += 1;
            }
            else if (mb.GetType() == typeof(Display))
            {
                mainDisplay = (Display)mb;
            }
        }
    }
}


