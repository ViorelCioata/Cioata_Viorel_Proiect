using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.Entity;
using System.Data;
using GameShopDatabaseModel.Properties;

namespace Cioata_Viorel_Proiect
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    enum ActionState
    {
        New,
        Edit,
        Delete,
        Nothing
    }


    public partial class MainWindow : Window
    {

        ActionState action = ActionState.Nothing;
        GameShopDatabaseEntitiesModel ctx = new GameShopDatabaseEntitiesModel();
        CollectionViewSource gamesViewSource;
        CollectionViewSource playersViewSource;
        CollectionViewSource panelViewSource;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            gamesViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("gameViewSource")));
            gamesViewSource.Source = ctx.Games.Local;

            playersViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("playerViewSource")));
            playersViewSource.Source = ctx.Players.Local;

            panelViewSource = (CollectionViewSource)FindResource("gameGamesPanelsViewSource");

            ctx.Games.Load();
            ctx.Players.Load();
            ctx.GamesPanels.Load();

            cmbGames.ItemsSource = ctx.Games.Local;
            //cmbGames.DisplayMemberPath = "name";
            cmbGames.SelectedValuePath = "gameId";

            cmbPlayers.ItemsSource = ctx.Players.Local;
            cmbPlayers.DisplayMemberPath = "gamerTag";
            cmbPlayers.SelectedValuePath = "playerId";

            BindDataGrid();
        }

        private void BindDataGrid()
        {
            var queryOrder = from panel in ctx.GamesPanels
                             join game in ctx.Games on panel.panelId equals
                             game.gameId
                             join player in ctx.Players on panel.playerId
                 equals player.playerId
                             select new
                             {
                                 panel.panelId,
                                 panel.playerId,
                                 panel.gameId,
                                 game.name,
                                 game.type,
                                 player.gamerTag
                             };
            panelViewSource.Source = queryOrder.ToList();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("now saving");
            Game game = null;
            if (action == ActionState.New)
            {
                MessageBox.Show("new");
                try
                {
                    game = new Game()
                    {
                        name = nameTextBox.Text.Trim(),
                        description = descriptionTextBox.Text.Trim(),
                        ageRestriction = int.Parse(ageRestrictionTextBox.Text.Trim()),
                        type = typeTextBox.Text.Trim()
                    };

                    ctx.Games.Add(game);
                    gamesViewSource.View.Refresh();

                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else if (action == ActionState.Edit)
            {
                MessageBox.Show("edit");
                try
                {
                    game = (Game)gameDataGrid.SelectedItem;
                    game.name = nameTextBox.Text.Trim();
                    game.description = descriptionTextBox.Text.Trim();
                    game.ageRestriction = int.Parse(ageRestrictionTextBox.Text.Trim());
                    game.type = typeTextBox.Text.Trim();

                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }

                gamesViewSource.View.Refresh();

                gamesViewSource.View.MoveCurrentTo(game);
            }
            else if (action == ActionState.Delete)
            {
                MessageBox.Show("delete");
                try
                {
                    game = (Game)gameDataGrid.SelectedItem;
                    ctx.Games.Remove(game);
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                gamesViewSource.View.Refresh();

            }
            action = ActionState.Nothing;
        }


        private void btnSave1_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("now saving");
            Player player = null;
            if (action == ActionState.New)
            {
                MessageBox.Show("New");
                try
                {
                    player = new Player()
                    {
                        age = int.Parse(ageTextBox.Text.Trim()),
                        name = nameTextBox1.Text.Trim(),
                        gamerTag = gamerTagTextBox.Text.Trim(),
                        gender = genderTextBox.Text.Trim()
                    };

                    ctx.Players.Add(player);
                    playersViewSource.View.Refresh();

                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else if (action == ActionState.Edit)
            {
                MessageBox.Show("Edit");
                try
                {
                    player = (Player)playerDataGrid.SelectedItem;
                    player.name = nameTextBox1.Text.Trim();
                    player.age = int.Parse(ageTextBox.Text.Trim());
                    player.gamerTag = gamerTagTextBox.Text.Trim();
                    player.gender = genderTextBox.Text.Trim();

                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }

                playersViewSource.View.Refresh();

                playersViewSource.View.MoveCurrentTo(player);
            }
            else if (action == ActionState.Delete)
            {
                MessageBox.Show("Delete");
                try
                {
                    player = (Player)playerDataGrid.SelectedItem;
                    ctx.Players.Remove(player);
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                playersViewSource.View.Refresh();
            }

            action = ActionState.Nothing;
        }

        private void btnSave2_Click(object sender, RoutedEventArgs e)
        {
            GamesPanel panel = null;
            if (action == ActionState.New)
            {
                try
                {
                    Game game = (Game)cmbGames.SelectedItem;
                    Player player = (Player)cmbPlayers.SelectedItem;
                    panel = new GamesPanel()
                    {
                        gameId = game.gameId,
                        playerId = player.playerId
                    };
                    ctx.GamesPanels.Add(panel);
                    panelViewSource.View.Refresh();
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }

                panelViewSource.View.Refresh();

            }
            else if (action == ActionState.Edit)
            {
                dynamic selectedAssignment = gamesPanelsDataGrid.SelectedItem;
                try
                {
                    int curr_id = selectedAssignment.assignmentId;
                    var editedPanel = ctx.GamesPanels.FirstOrDefault(s => s.panelId == curr_id);
                    if (editedPanel != null)
                    {
                        editedPanel.gameId = Int32.Parse(cmbGames.SelectedValue.ToString());
                        editedPanel.playerId = Convert.ToInt32(cmbPlayers.SelectedValue.ToString());
                        ctx.SaveChanges();
                    }
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                BindDataGrid();
                panelViewSource.View.MoveCurrentTo(selectedAssignment);

                panelViewSource.View.Refresh();
            }
            else if (action == ActionState.Delete)
            {
                try
                {
                    dynamic selectedAssignment = gamesPanelsDataGrid.SelectedItem;
                    int curr_id = selectedAssignment.assignmentId;
                    var deletedPanel = ctx.GamesPanels.FirstOrDefault(s => s.panelId == curr_id);
                    if (deletedPanel != null)
                    {
                        ctx.GamesPanels.Remove(deletedPanel);
                        ctx.SaveChanges();
                        MessageBox.Show("Assignment Deleted Successfully", "Message");
                        BindDataGrid();
                    }
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            action = ActionState.Nothing;
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Edit;

            BindingOperations.ClearBinding(ageRestrictionTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(nameTextBox, TextBox.TextProperty);
            SetValidationBinding();
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.New;
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Delete;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            nameTextBox.Text = "";
            descriptionTextBox.Text = "";
            ageRestrictionTextBox.Text = "";
            typeTextBox.Text = "";

        }


        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            gamesViewSource.View.MoveCurrentToNext();
        }

        private void btnPrevious_Click(object sender, RoutedEventArgs e)
        {
            gamesViewSource.View.MoveCurrentToPrevious();
        }

        private void btnNext1_Click(object sender, RoutedEventArgs e)
        {
            playersViewSource.View.MoveCurrentToNext();
        }

        private void btnPrevious1_Click(object sender, RoutedEventArgs e)
        {
            playersViewSource.View.MoveCurrentToPrevious();
        }

        private void btnNext2_Click(object sender, RoutedEventArgs e)
        {
            panelViewSource.View.MoveCurrentToNext();
        }

        private void btnPrevious2_Click(object sender, RoutedEventArgs e)
        {
            panelViewSource.View.MoveCurrentToPrevious();
        }


        private void SetValidationBinding()
        {
            Binding gamesAgeRestrictionValidationBinding = new Binding();
            gamesAgeRestrictionValidationBinding.Source = gamesViewSource;
            gamesAgeRestrictionValidationBinding.Path = new PropertyPath("ageRestriction");
            gamesAgeRestrictionValidationBinding.NotifyOnValidationError = true;
            gamesAgeRestrictionValidationBinding.Mode = BindingMode.TwoWay;
            gamesAgeRestrictionValidationBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            gamesAgeRestrictionValidationBinding.ValidationRules.Add(new StringNotEmpty());
            ageRestrictionTextBox.SetBinding(TextBox.TextProperty, gamesAgeRestrictionValidationBinding);


            Binding gamesNameValidationBinding = new Binding();
            gamesNameValidationBinding.Source = gamesViewSource;
            gamesNameValidationBinding.Path = new PropertyPath("name");
            gamesNameValidationBinding.NotifyOnValidationError = true;
            gamesNameValidationBinding.Mode = BindingMode.TwoWay;
            gamesNameValidationBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            gamesNameValidationBinding.ValidationRules.Add(new StringMinLengthValidator());
            nameTextBox.SetBinding(TextBox.TextProperty, gamesNameValidationBinding); //setare binding nou
        }


    }
}
