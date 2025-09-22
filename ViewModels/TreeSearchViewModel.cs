using System.Collections.ObjectModel;
using System.Linq;
using DynamicData;


namespace AvaloniaTest.ViewModels;

public class TreeSearchViewModel : ViewModelBase
{
    public ObservableCollection<NodeBase> Nodes { get; }
    public ObservableCollection<NodeBase> SelectedNodes { get; }

    public TreeSearchViewModel()
    {
        SelectedNodes = new ObservableCollection<NodeBase>();
        Nodes = new ObservableCollection<NodeBase>
        {
            new OpenEditorNode("Dev Control", "IconGameController"),
            new OpenEditorNode("Zone Editor", "IconGameController"),
            new OpenEditorNode("Ground Editor", "IconGameController"),
            new NodeBase("Constants")
            {
                SubNodes = new ObservableCollection<NodeBase>
                {
                    new NodeBase("Start Parameters"),
                    new NodeBase("Universal Events"),
                    new NodeBase("Strings", "IconFloppyDisk")
                    {
                        SubNodes = new ObservableCollection<NodeBase>
                        {
                            new NodeBase("English"),
                            new NodeBase("Chinese")
                        }
                    },
                    new NodeBase("Effects")
                    {
                        SubNodes = new ObservableCollection<NodeBase>
                        {
                            new NodeBase("Heal FX"),
                            new NodeBase("+Charge FX"),
                        }
                    },
                }
            },
            new NodeBase("Data", "IconFloppyDisk")
            {
                SubNodes =
                {
                    new DataRootNode("Monsters", "Monsters", "IconGhost")
                    {
                        SubNodes = 
                        {
                            new DataItemNode("eevee", "eevee: Eevee"),
                            new DataItemNode("seviper", "seviper: Seviper")
                        }
                    },
                    new ActionDataNode("Items")
                    {
                        SubNodes = new ObservableCollection<NodeBase>
                        {
                            new NodeBase("ammo_cacnea_spike: Eevee"),
                        }
                    },
                    new ActionDataNode("Zones", "IconSword")
                    {
                        SubNodes = new ObservableCollection<NodeBase>
                        {
                            new NodeBase("ambush_forest: Ambush Forest"),
                        }
                    },
                    new ActionDataNode("Statuses", "IconHeart")
                    {
                        SubNodes = new ObservableCollection<NodeBase>
                        {
                            new NodeBase("para: Paralyzed"),
                        }
                    },
                }
            },
            new NodeBase("Sprites", "IconPaintBrush")
            {
                SubNodes = new ObservableCollection<NodeBase>
                {
                    new NodeBase("Char Sprites"),
                    new NodeBase("Portraits"),
                    new ActionDataNode("Particles")
                    {
                        SubNodes = new ObservableCollection<NodeBase>
                        {
                            new NodeBase("Absorb"),
                            new NodeBase("Acid_Blue"),
                        }
                    },
                    new ActionDataNode("Beam")
                    {
                        SubNodes = new ObservableCollection<NodeBase>
                        {
                            new NodeBase("Beam_2"),
                            new NodeBase("Beam_Pink"),
                        }
                    },
                }
            },
            new NodeBase("Mods", "IconPaintBrush")
            {
                SubNodes = new ObservableCollection<NodeBase>
                {
                    new NodeBase("halcyon: Halcyon"),
                    new NodeBase("zorea_mystery_dungeon: Zorea Mystery Dungeon"),
                }
            }
        };
        var moth = Nodes.Last().SubNodes?.Last();
        if (moth != null) SelectedNodes.Add(moth);
    }
}