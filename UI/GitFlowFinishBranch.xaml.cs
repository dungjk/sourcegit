using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace SourceGit.UI {

    /// <summary>
    ///     Confirm finish git-flow branch dialog
    /// </summary>
    public partial class GitFlowFinishBranch : UserControl {
        private Git.Repository repo = null;
        private Git.Branch branch = null;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="repo"></param>
        /// <param name="branch"></param>
        public GitFlowFinishBranch(Git.Repository repo, Git.Branch branch) {
            this.repo = repo;
            this.branch = branch;

            InitializeComponent();

            switch (branch.Kind) {
            case Git.Branch.Type.Feature:
                txtTitle.Content = "Git Flow - Finish Feature";
                txtBranchType.Content = "Feature :";
                break;
            case Git.Branch.Type.Release:
                txtTitle.Content = "Git Flow - Finish Release";
                txtBranchType.Content = "Release :";
                break;
            case Git.Branch.Type.Hotfix:
                txtTitle.Content = "Git Flow - Finish Hotfix";
                txtBranchType.Content = "Hotfix :";
                break;
            default:
                PopupManager.Close();
                return;
            }

            txtBranchName.Content = branch.Name;
        }

        /// <summary>
        ///     Show this dialog.
        /// </summary>
        /// <param name="repo"></param>
        /// <param name="branch"></param>
        public static void Show(Git.Repository repo, Git.Branch branch) {
            PopupManager.Show(new GitFlowFinishBranch(repo, branch));
        }

        /// <summary>
        ///     Do finish
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Sure(object sender, RoutedEventArgs e) {
            PopupManager.Lock();

            DoubleAnimation anim = new DoubleAnimation(0, 360, TimeSpan.FromSeconds(1));
            anim.RepeatBehavior = RepeatBehavior.Forever;
            statusIcon.RenderTransform.BeginAnimation(RotateTransform.AngleProperty, anim);
            status.Visibility = Visibility.Visible;

            await Task.Run(() => repo.FinishGitFlowBranch(branch));

            status.Visibility = Visibility.Collapsed;
            statusIcon.RenderTransform.BeginAnimation(RotateTransform.AngleProperty, null);
            PopupManager.Close(true);
        }

        /// <summary>
        ///     Cancel finish
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cancel(object sender, RoutedEventArgs e) {
            PopupManager.Close();
        }
    }
}
