namespace GithubForOutlook.Logic.Ribbons.Tasks
{
	using System;
	using System.Windows;

	using GithubForOutlook.Logic.Modules.Notifications;
	using GithubForOutlook.Logic.Modules.Settings;
	using GithubForOutlook.Logic.Modules.Tasks;

	using Microsoft.Office.Core;
	using Microsoft.Office.Interop.Outlook;

	using VSTOContrib.Core.RibbonFactory;
	using VSTOContrib.Core.RibbonFactory.Interfaces;
	using VSTOContrib.Core.RibbonFactory.Internal;
	using VSTOContrib.Core.Wpf;
	using VSTOContrib.Outlook.RibbonFactory;

	[RibbonViewModel(OutlookRibbonType.OutlookTask)]
    public class GithubTask : OfficeViewModelBase, IRibbonViewModel, IRegisterCustomTaskPane
    {
        
        private bool panelShown;
        private ICustomTaskPaneWrapper githubTaskPane;
        private GithubTaskAdapter githubIssue;

        public GithubTask(TasksViewModel tasks, NotificationsViewModel notifications, SettingsViewModel settings)
        {
            this.Tasks = tasks;
            this.Notifications = notifications;
            this.Settings = settings;
        }

        public void Initialised(object context)
        {
            var task = (TaskItem)context;
            this.githubIssue = new GithubTaskAdapter(task);
        }

        public void CurrentViewChanged(object currentView)
        {
        }

        public TasksViewModel Tasks { get; private set; }
        public NotificationsViewModel Notifications { get; private set; }
        public SettingsViewModel Settings { get; private set; }

        public bool IsGithubTask
        {
            get { return this.githubIssue.IsGithubTask; }
            private set
            {
                //githubIssue.IsGithubTask = value;
                this.RaisePropertyChanged(() => this.IsGithubTask);
            }
        }

        public bool PanelShown
        {
            get { return this.panelShown; }
            set
            {
                if (this.panelShown == value) return;
                this.panelShown = value;
                this.githubTaskPane.Visible = value;
                this.RaisePropertyChanged("PanelShown");
            }
        }

        public void CreateIssue(IRibbonControl control)
        {
            MessageBox.Show("Hai");
        }

        public void RegisterTaskPanes(Register register)
        {
            this.githubTaskPane = register(() => new WpfPanelHost
            {
                Child = new GithubTaskPanel
                {
                    DataContext = this
                }
            }, "Github");
            this.githubTaskPane.Visible = this.IsGithubTask;
            this.PanelShown = this.IsGithubTask;
            this.githubTaskPane.VisibleChanged += this.GithubTaskPaneVisibleChanged;
            this.GithubTaskPaneVisibleChanged(this, EventArgs.Empty);
        }

        public void Cleanup()
        {
            this.githubTaskPane.VisibleChanged -= this.GithubTaskPaneVisibleChanged;
        }

        public IRibbonUI RibbonUi { get; set; }

        private void GithubTaskPaneVisibleChanged(object sender, EventArgs e)
        {
            this.panelShown = this.githubTaskPane.Visible;
            this.RaisePropertyChanged("PanelShown");
        }
    }
}