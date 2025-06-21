const fs = require("fs");
const path = require("path");

const DATA_FOLDER = "../../DataFiles";
const BackgroundImagePath = path.join("./src", "EmptyBackground.png");
const ConfigPath = "./Config.cfg";

(async () => {
    if (!fs.existsSync("./src")) fs.mkdirSync("./src");
    if (!fs.existsSync(BackgroundImagePath)) fs.copyFileSync(path.join(DATA_FOLDER, "EmptyBackground.png"), BackgroundImagePath);
    if (!fs.existsSync(ConfigPath)) fs.copyFileSync(path.join(DATA_FOLDER, "Config.cfg"), ConfigPath);
    fs.unlinkSync("build.js");
})()

console.log("Build completed.");
