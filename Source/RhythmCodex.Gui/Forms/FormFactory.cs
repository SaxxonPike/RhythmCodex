using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using RhythmCodex.Cli;
using RhythmCodex.Gui.FluentForms;
using RhythmCodex.IoC;

namespace RhythmCodex.Gui.Forms
{
    [Service]
    public class FormFactory : IFormFactory
    {
        private readonly IConsoleEventSource _consoleEventSource;
        private readonly IFileDialog _fileDialog;
        private readonly IGuiTasks _guiTasks;
        private readonly IControlFactory _controlFactory;
        private readonly IAppProgressTracker _appProgressTracker;

        private static class Ids
        {
            private static string Id(string baseName) => baseName + Guid.NewGuid().ToString();

            public static readonly string LogTabId = Id(nameof(LogTabId));
            public static readonly string MainFormOutputFolderSelect = Id(nameof(MainFormOutputFolderSelect));
            public static readonly string DdrExtract573GamePath = Id(nameof(DdrExtract573GamePath));
            public static readonly string DdrExtract573CardPath = Id(nameof(DdrExtract573CardPath));
            public static readonly string DdrExtract573Start = Id(nameof(DdrExtract573Start));
            public static readonly string MainForm = Id(nameof(MainForm));
            public static readonly string MainFormMenu = Id(nameof(MainFormMenu));
            public static readonly string MainFormTabControl = Id(nameof(MainFormTabControl));
            public static readonly string Log = Id(nameof(Log));
            public static readonly string DdrDecrypt573AudioPath = Id(nameof(DdrDecrypt573AudioPath));
            public static readonly string DdrDecrypt573AudioStart = Id(nameof(DdrDecrypt573AudioStart));
            public static readonly string DdrDecrypt573AudioRename = Id(nameof(DdrDecrypt573AudioRename));
            public static readonly string SsqDecodePath = Id(nameof(SsqDecodePath));
            public static readonly string SsqDecodeStart = Id(nameof(SsqDecodeStart));
            public static readonly string SsqDecodeOffset = Id(nameof(SsqDecodeOffset));
            public static readonly string BeatmaniaDecodeDjmainHddPath = Id(nameof(BeatmaniaDecodeDjmainHddPath));
            public static readonly string BeatmaniaDecodeDjmainHddStart = Id(nameof(BeatmaniaDecodeDjmainHddStart));
            public static readonly string ProgressTable = Id(nameof(ProgressTable));
            public static readonly string BeatmaniaDecodeDjmainHddSkipAudio = Id(nameof(BeatmaniaDecodeDjmainHddSkipAudio));
            public static readonly string BeatmaniaDecodeDjmainHddSkipCharts = Id(nameof(BeatmaniaDecodeDjmainHddSkipCharts));
            public static readonly string BeatmaniaDecodeDjmainHddRawCharts = Id(nameof(BeatmaniaDecodeDjmainHddRawCharts));
        }

        public FormFactory(IConsoleEventSource consoleEventSource, IFileDialog fileDialog, IGuiTasks guiTasks,
            IControlFactory controlFactory, IAppProgressTracker appProgressTracker)
        {
            _consoleEventSource = consoleEventSource;
            _fileDialog = fileDialog;
            _guiTasks = guiTasks;
            _controlFactory = controlFactory;
            _appProgressTracker = appProgressTracker;
        }

        public Form CreateMainForm()
        {
            var progressTimer = new Timer
            {
                Interval = 200
            };

            var form = new FluentForm
            {
                Id = Ids.MainForm,
                Text = "RhythmCodex GUI",
                Controls = new List<FluentControl>
                {
                    CreateMainFormTabs(),
                    CreateMainFormMenu(),
                },
                AfterBuild = x =>
                {
                    x.Control.MainMenuStrip = x.GetControl<MenuStrip>(Ids.MainFormMenu);
                    progressTimer.Tick += (s, e) => UpdateProgress(x.GetControl<TableLayoutPanel>(Ids.ProgressTable));
                }
            };

            var builtForm = (Form) form.Build();
            builtForm.MinimumSize = builtForm.Size = new Size(640, 480);
            builtForm.Closing += (s, e) => progressTimer.Dispose();

            progressTimer.Start();

            return builtForm;
        }

        private void UpdateProgress(TableLayoutPanel tab)
        {
            var statuses = _appProgressTracker.GetAll().ToList();

            var existingStatuses = statuses
                .SelectMany(s => tab.Controls.OfType<Control>().Where(c => c.Name.StartsWith(s.Id)))
                .ToList();

            var controlsToRemove = tab.Controls.OfType<Control>().Except(existingStatuses)
                .ToList();

            var tasksToAdd = statuses.Where(s => !existingStatuses.Select(c => c.Name).Contains(s.Id))
                .ToList();

            var newControls = tasksToAdd
                .SelectMany(p => _controlFactory.CreateProgress(p.Id, p.Name))
                .Select(fc => fc.Build())
                .ToArray();

            tab.SuspendLayout();

            foreach (var control in controlsToRemove)
                tab.Controls.Remove(control);

            tab.Controls.AddRange(newControls);
            var controls = tab.Controls;
            
            foreach (var status in statuses)
            {
                var bar = controls.OfType<ProgressBar>().Single(c => c.Name == status.Id);
                bar.Minimum = 0;
                bar.Maximum = 100;
                bar.Value = (int) (status.Progress * 100);
                
                var message = controls.OfType<Label>().Single(c => c.Name == status.Id + "Message");
                message.Text = status.Message;
                
                var percent = controls.OfType<Label>().Single(c => c.Name == status.Id + "Percent");
                percent.Text = $"{(int)(status.Progress * 100)}%";
            }

            tab.ResumeLayout();
        }

        private FluentControl CreateMainFormLog()
        {
            var table = _controlFactory.CreateStandardTable(Ids.ProgressTable, new FluentControl[] { }, false);
            return table;
        }

        private FluentControl CreateMainFormMenu()
        {
            return new FluentMenu
            {
                Id = Ids.MainFormMenu,
                Items = new List<FluentMenuItem>
                {
                    new FluentMenuItem
                    {
                        Text = "&File",
                        Items = new List<FluentMenuItem>
                        {
                            new FluentMenuItem
                            {
                                Text = "E&xit",
                                OnClick = Application.Exit
                            }
                        }
                    }
                }
            };
        }

        private FluentControl CreateMainFormTabs()
        {
            return new FluentPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(8, 2, 8, 8),
                Controls = new List<FluentControl>
                {
                    _controlFactory.CreateStandardTable(null,
                        _controlFactory.CreateFolderSelect(
                                Ids.MainFormOutputFolderSelect, "Output Path")
                            .Concat(new[] {CreateMainFormTabControl()}), true)
                }
            };
        }

        private FluentControl CreateMainFormTabControl()
        {
            return new FluentTabControl
            {
                Dock = DockStyle.Fill,
                Id = Ids.MainFormTabControl,
                ColumnSpan = 3,
                Items = new List<FluentTabPage>
                {
                    new FluentTabPage
                    {
                        Id = Ids.LogTabId,
                        Text = "Log",
                        Controls = new List<FluentControl>
                        {
                            CreateMainFormLog()
                        }
                    },
                    new FluentTabPage
                    {
                        Text = "BMS",
                        Controls = new List<FluentControl>
                        {
                            CreateMainFormBmsPage(),
                        }
                    },
                    new FluentTabPage
                    {
                        Text = "beatmania",
                        Controls = new List<FluentControl>
                        {
                            CreateMainFormBeatmaniaPage()
                        }
                    },
                    new FluentTabPage
                    {
                        Text = "DanceDanceRevolution",
                        Controls = new List<FluentControl>
                        {
                            CreateMainFormDanceDanceRevolutionPage()
                        }
                    }
                }
            };
        }

        private FluentControl CreateMainFormBmsPage()
        {
            return new FluentPanel();
        }

        private FluentControl CreateMainFormDanceDanceRevolutionPage()
        {
            return new FluentTabControl
            {
                Items = new List<FluentTabPage>
                {
                    new FluentTabPage
                    {
                        Text = "Common",
                        AutoScroll = true,
                        Controls = new List<FluentControl>
                        {
                            CreateMainFormDanceDanceRevolutionCommonPage()
                        }
                    },
                    new FluentTabPage
                    {
                        Text = "Playstation"
                    },
                    new FluentTabPage
                    {
                        Text = "Playstation 2"
                    },
                    new FluentTabPage
                    {
                        Text = "Xbox"
                    },
                    new FluentTabPage
                    {
                        Text = "Xbox 360"
                    },
                    new FluentTabPage
                    {
                        Text = "System 573",
                        AutoScroll = true,
                        Controls = new List<FluentControl>
                        {
                            CreateMainFormDanceDanceRevolution573Page()
                        }
                    },
                    new FluentTabPage
                    {
                        Text = "Python"
                    },
                    new FluentTabPage
                    {
                        Text = "PC"
                    }
                }
            };
        }

        private FluentControl CreateMainFormDanceDanceRevolutionCommonPage()
        {
            return new FluentPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = false,
                AutoScroll = true,
                Controls = new List<FluentControl>
                {
                    _controlFactory.CreateStandardTable(null,
                        _controlFactory.CreateSpacer("Convert SSQ to SM/SSC")
                            .Concat(_controlFactory
                                .CreateFileSelect(Ids.SsqDecodePath, "SSQ file", true))
                            .Concat(_controlFactory
                                .CreateTextEntry(Ids.SsqDecodeOffset, "#OFFSET adjust"))
                            .Concat(_controlFactory
                                .CreateBigButton(Ids.SsqDecodeStart, "Go",
                                    SsqDecode))
                        , false)
                }
            };
        }
        
        private FluentControl CreateMainFormDanceDanceRevolution573Page()
        {
            return new FluentPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = false,
                AutoScroll = true,
                Controls = new List<FluentControl>
                {
                    _controlFactory.CreateStandardTable(null,
                        _controlFactory.CreateSpacer("Extract files from flash card images")
                            .Concat(_controlFactory
                                .CreateFileSelect(Ids.DdrExtract573GamePath, "GAME file", false))
                            .Concat(_controlFactory
                                .CreateFileSelect(Ids.DdrExtract573CardPath, "CARD file (optional)", false))
                            .Concat(_controlFactory
                                .CreateBigButton(Ids.DdrExtract573Start, "Go",
                                    DdrExtract573Flash))
                            .Concat(_controlFactory.CreateSpacer("Decrypt MP3 files from the disc"))
                            .Concat(_controlFactory
                                .CreateFileSelect(Ids.DdrDecrypt573AudioPath, "Encrypted audio files", true))
                            .Concat(_controlFactory
                                .CreateCheckbox(Ids.DdrDecrypt573AudioRename, "Decode song names"))
                            .Concat(_controlFactory
                                .CreateBigButton(Ids.DdrDecrypt573AudioStart, "Go",
                                    DdrDecrypt573Audio))
                        , false)
                }
            };
        }

        private FluentControl CreateMainFormBeatmaniaPage()
        {
            return new FluentTabControl
            {
                Items = new List<FluentTabPage>
                {
                    new FluentTabPage
                    {
                        Text = "Common"
                    },
                    new FluentTabPage
                    {
                        Text = "Playstation"
                    },
                    new FluentTabPage
                    {
                        Text = "Playstation 2"
                    },
                    new FluentTabPage
                    {
                        Text = "Djmain",
                        AutoScroll = true,
                        Controls = new List<FluentControl>
                        {
                            CreateMainFormBeatmaniaDjmainPage()
                        }
                    },
                    new FluentTabPage
                    {
                        Text = "Twinkle"
                    },
                    new FluentTabPage
                    {
                        Text = "Firebeat"
                    },
                    new FluentTabPage
                    {
                        Text = "PC"
                    }
                }
            };
        }

        private FluentControl CreateMainFormBeatmaniaDjmainPage()
        {
            return new FluentPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = false,
                AutoScroll = true,
                Controls = new List<FluentControl>
                {
                    _controlFactory.CreateStandardTable(null,
                        _controlFactory.CreateSpacer("Convert Djmain HDD to BMS")
                            .Concat(_controlFactory
                                .CreateFileSelect(Ids.BeatmaniaDecodeDjmainHddPath, "HDD image", false))
                            .Concat(_controlFactory
                                .CreateDualCheckbox(Ids.BeatmaniaDecodeDjmainHddSkipAudio, "Disable audio", 
                                    Ids.BeatmaniaDecodeDjmainHddSkipCharts, "Disable charts"))
                            .Concat(_controlFactory
                                .CreateCheckbox(Ids.BeatmaniaDecodeDjmainHddRawCharts, "Enable raw charts"))
                            .Concat(_controlFactory
                                .CreateBigButton(Ids.BeatmaniaDecodeDjmainHddStart, "Go",
                                    BeatmaniaDecodeDjmainHdd))
                        , false)
                }
            };
        }

        private static void ShowLogTab(FluentContext context)
        {
            var page = context.GetControl<TabPage>(Ids.LogTabId);
            var tabs = context.GetControl<TabControl>(Ids.MainFormTabControl);
            tabs.SelectedTab = page;
        }

        private void DdrExtract573Flash(FluentContext context)
        {
            ShowLogTab(context);
            _guiTasks.DdrExtract573Flash(
                context.GetControl<TextBox>(Ids.DdrExtract573GamePath).Text,
                context.GetControl<TextBox>(Ids.DdrExtract573CardPath).Text,
                context.GetControl<TextBox>(Ids.MainFormOutputFolderSelect).Text);
        }

        private void DdrDecrypt573Audio(FluentContext context)
        {
            ShowLogTab(context);
            _guiTasks.DdrDecrypt573Audio(
                context.GetControl<TextBox>(Ids.DdrDecrypt573AudioPath).Text,
                context.GetControl<TextBox>(Ids.MainFormOutputFolderSelect).Text,
                context.GetControl<CheckBox>(Ids.DdrDecrypt573AudioRename).Checked);
        }

        private void SsqDecode(FluentContext context)
        {
            ShowLogTab(context);
            var offsetArg = double.TryParse(context.GetControl<TextBox>(Ids.SsqDecodeOffset).Text, out var offset)
                ? offset : 0;
            _guiTasks.SsqDecode(
                context.GetControl<TextBox>(Ids.SsqDecodePath).Text,
                context.GetControl<TextBox>(Ids.MainFormOutputFolderSelect).Text,
                offsetArg);
        }

        private void BeatmaniaDecodeDjmainHdd(FluentContext context)
        {
            ShowLogTab(context);
            _guiTasks.BeatmaniaDecodeDjmainHdd(
                context.GetControl<TextBox>(Ids.BeatmaniaDecodeDjmainHddPath).Text,
                context.GetControl<TextBox>(Ids.MainFormOutputFolderSelect).Text,
                context.GetControl<CheckBox>(Ids.BeatmaniaDecodeDjmainHddSkipAudio).Checked,
                context.GetControl<CheckBox>(Ids.BeatmaniaDecodeDjmainHddSkipCharts).Checked,
                context.GetControl<CheckBox>(Ids.BeatmaniaDecodeDjmainHddRawCharts).Checked);
        }
    }
}