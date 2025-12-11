/**
* <author>Christophe Roblin</author>
* <email>lifxmod@gmail.com</email>
* <url>lifxmod.com</url>
* <credits>Jorvik for creating the original modification</credits>
* <description>Jorvik mod introduced to be lifx and yolauncher compatible, with robust rule display and GUI-triggered rule requests</description>
* <license>GNU GENERAL PUBLIC LICENSE Version 3, 29 June 2007</license>
*/

if (!isObject(JorvikMod2))
{
    new ScriptObject(JorvikMod2) {};
}

package JorvikMod2
{
    function JorvikMod2::setup()
    {
        echo("JorvikMod2 setup called!");

        LiFx::registerCallback($LiFx::hooks::onMaterialsLoad, RegisterMaterials, JorvikMod2);
        LiFx::registerCallback($LiFx::hooks::onInitialized, onInitialized, JorvikMod2);

        // Copy heraldry once the GUI and assets are initialized
        LiFx::registerCallback($LiFx::hooks::onInitialized, copyAllHeraldry, JorvikMod2);

        LiFx::registerCallback($LiFx::hooks::onDatablockLoad, RegisterDatablock, JorvikMod2);

        $JorvikMod2::RulesBuffer = "";
    }

    function JorvikMod2::RegisterMaterials()
    {
        LiFx::loadRecursivelyInFolder("yolauncher/modpack/mods/Jorvik2/", "materials.cs");
    }

    function JorvikMod2::path()
    {
        %path = $Con::File;
        echo(%path);
        return %path;
    }

    function JorvikMod2::RegisterDatablock()
    {
        LiFx::loadRecursivelyInFolder("yolauncher/modpack/mods/Jorvik2/art/datablocks", "Transport.cs");
        LiFx::loadRecursivelyInFolder("yolauncher/modpack/mods/Jorvik2/art/datablocks", "audioProfiles.cs");
    }

    //--------------------------------------------------------------------------
    //  RECURSIVE COPY FUNCTION
    //--------------------------------------------------------------------------

    function JorvikMod2::copyHeraldryRecursively(%this, %source, %dest)
    {
        echo("Copying PNGs: " @ %source @ " -> " @ %dest);

        // Ensure destination exists
        if (!isDirectory(%dest))
            createPath(%dest);

        // Copy PNG files in this folder
        %pattern = %source @ "/*.png";
        %file = findFirstFile(%pattern);

        while (%file !$= "")
        {
            %name   = fileName(%file);
            %target = %dest @ "/" @ %name;

            echo(" - Copying: " @ %file @ " -> " @ %target);

            fileCopy(%file, %target, true); // overwrite = true

            %file = findNextFile(%pattern);
        }

        // Process subfolders
        %subPattern = %source @ "/*";
        %sub = findFirstFile(%subPattern);

        while (%sub !$= "")
        {
            if (isDirectory(%sub))
            {
                %folderName = fileName(%sub);

                if (%folderName !$= "" && %folderName !$= "." && %folderName !$= "..")
                {
                    %newSrc = %sub;
                    %newDst = %dest @ "/" @ %folderName;

                    %this.copyHeraldryRecursively(%newSrc, %newDst);
                }
            }

            %sub = findNextFile(%subPattern);
        }
    }

    //--------------------------------------------------------------------------
    //  CALLBACK WRAPPER TO START COPYING
    //--------------------------------------------------------------------------

    function JorvikMod2::copyAllHeraldry(%this)
    {
        %src = "yolauncher/modpack/mods/Jorvik2/art/Textures/Heraldry";
        %dst = expandFilename("./art/Textures/Heraldry");

        echo("Starting Heraldry COPY...");
        %this.copyHeraldryRecursively(%src, %dst);
        echo("Heraldry COPY Complete.");
    }

    //--------------------------------------------------------------------------
    //  GUI INITIALIZATION
    //--------------------------------------------------------------------------

    function JorvikMod2::onInitialized()
    {
        if (isObject(MainMenuGui))
            MainMenuGui.delete();
        if (isObject(SettingsMenuGui))
            SettingsMenuGui.delete();
        if (isObject(selectCharacterDlg))
            selectCharacterDlg.delete();

        LiFx::loadRecursivelyInFolder("yolauncher/modpack/mods/Jorvik2/art/gui/forms", "LiFxheraldryDialog.gui");
        LiFx::loadRecursivelyInFolder("yolauncher/modpack/mods/Jorvik2/art/gui/forms", "LiFxmainMenuGui.gui");
        LiFx::loadRecursivelyInFolder("yolauncher/modpack/mods/Jorvik2/art/gui/forms", "LiFxselectCharacter.gui");
        LiFx::loadRecursivelyInFolder("yolauncher/modpack/mods/Jorvik2/art/gui/forms", "LiFxloadingGui.gui");
    }

    //--------------------------------------------------------------------------
    //  RULES SYSTEM
    //--------------------------------------------------------------------------

    function clientCmdDisplayRules(%chunk)
    {
        if (!isDefined("$JorvikMod2::RulesBuffer"))
            $JorvikMod2::RulesBuffer = "";

        $JorvikMod2::RulesBuffer = $JorvikMod2::RulesBuffer @ %chunk;
    }

    function clientCmdEndRulesTransmission()
    {
        if ($JorvikMod2::RulesBuffer $= "")
        {
            echo("No rules received from server.");
            return;
        }

        echo("Rules received:\n" @ $JorvikMod2::RulesBuffer);
        messageBoxOK("Server Rules", $JorvikMod2::RulesBuffer);

        $JorvikMod2::RulesBuffer = "";
    }

    function displayRules(%request)
    {
        if (%request)
        {
            echo("Requesting rules from server...");
            commandToServer('RequestRules');
        }
    }
};

activatePackage(JorvikMod2);
LiFx::registerCallback($LiFx::hooks::mods, setup, JorvikMod2);
