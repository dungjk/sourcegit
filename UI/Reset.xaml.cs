﻿using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace SourceGit.UI {

    /// <summary>
    ///     Reset branch to revision dialog.
    /// </summary>
    public partial class Reset : UserControl {
        private Git.Repository repo = null;
        private string revision = null;

        /// <summary>
        ///     Reset mode.
        /// </summary>
        public class Mode {
            public Brush Color { get; set; }
            public string Name { get; set; }
            public string Desc { get; set; }
            public string Arg { get; set; }
            public Mode(Brush b, string n, string d, string a) {
                Color = b;
                Name = n;
                Desc = d;
                Arg = a;
            }
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="opened"></param>
        /// <param name="current"></param>
        /// <param name="commit"></param>
        public Reset(Git.Repository opened, Git.Branch current, Git.Commit commit) {
            InitializeComponent();

            repo = opened;
            revision = commit.SHA;

            branch.Content = current.Name;
            desc.Content = $"{commit.ShortSHA}  {commit.Subject}";
            combMode.ItemsSource = new Mode[] {
                new Mode(Brushes.Green, "Soft", "Keep all changes. Stage differences", "--soft"),
                new Mode(Brushes.Yellow, "Mixed", "Keep all changes. Unstage differences", "--mixed"),
                new Mode(Brushes.Red, "Hard", "Discard all changes", "--hard"),
            };
            combMode.SelectedIndex = 0;
        }

        /// <summary>
        ///     Show dialog.
        /// </summary>
        /// <param name="repo"></param>
        /// <param name="commit"></param>
        public static void Show(Git.Repository repo, Git.Commit commit) {
            var current = repo.CurrentBranch();
            if (current == null) return;

            PopupManager.Show(new Reset(repo, current, commit));
        }

        /// <summary>
        ///     Start reset.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Start(object sender, RoutedEventArgs e) {
            var mode = combMode.SelectedItem as Mode;
            if (mode == null) return;

            PopupManager.Lock();

            DoubleAnimation anim = new DoubleAnimation(0, 360, TimeSpan.FromSeconds(1));
            anim.RepeatBehavior = RepeatBehavior.Forever;
            statusIcon.RenderTransform.BeginAnimation(RotateTransform.AngleProperty, anim);
            status.Visibility = Visibility.Visible;

            await Task.Run(() => repo.Reset(revision, mode.Arg));

            status.Visibility = Visibility.Collapsed;
            statusIcon.RenderTransform.BeginAnimation(RotateTransform.AngleProperty, null);
            PopupManager.Close(true);
        }

        /// <summary>
        ///     Cancel.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cancel(object sender, RoutedEventArgs e) {
            PopupManager.Close();
        }
    }
}
