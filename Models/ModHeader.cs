using System.Collections.Generic;

namespace AvaloniaTest.Models;

public class ModHeader
{
    public string Name { get; set; }
    public string Key { get; set; }

    public static readonly List<ModHeader> Mods = new List<ModHeader>()
    {
        new ModHeader("[NULL]", "origin"),
        new ModHeader("Super Party Bros", "Super_Party_Bros"),
        new ModHeader("Resource Dungeon Pack", "source_duns_imbi"),
        new ModHeader("Trio's Sandbox", "trios_sandbox"),
        new ModHeader("Bubsy Mystery Dungeon", "bubsy_md"),
        new ModHeader("Friend Area", "friend_area"),
        new ModHeader("Typeless Moves", "typeless_moves"),
        new ModHeader("Bossfights", "bossfights"),
        new ModHeader("Explorers of Friending", "explorers_of_friending"),
        new ModHeader("All Starters", "starter_mod"),
        new ModHeader("Visible Monster Houses", "visible_monster_houses"),
        new ModHeader("Project EoN (0.2.3)", "what_does_n_stand_for"),
        new ModHeader("Explorers of Kanto", "explorers_of_kanto"),
        new ModHeader("Halcyon", "halcyon"),
        new ModHeader("Ruin", "ruins"),
        new ModHeader("More Items", "more_items"),
        new ModHeader("Enable Mission Board", "enable_mission_board"),
        new ModHeader("Silver Resistance", "silver_resistance"),
        new ModHeader("Rev Mod", "rev_mod"),
        new ModHeader("Halcyon No Nickname", "halcyon_nickname"),
        new ModHeader("Mystery Ruins", "mystery_ruins"),
        new ModHeader("Trio's Sandbox", "trios_sandbox_pack"),
        new ModHeader("Music Notice", "music_notice"),
        new ModHeader("Trio's Dungeon Pack", "trios_dungeon_pack"),
        new ModHeader("Raw", ""),
        new ModHeader("Gender Unlock", "gender_unlock"),
    };

    public ModHeader(string name, string key)
    {
        Name = name;
        Key = key;
    }
}