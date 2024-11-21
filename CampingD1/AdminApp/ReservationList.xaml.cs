using Database.Types;

namespace AdminApp;

public partial class ReservationList : ContentPage
{
    public ReservationList()
    {
        InitializeComponent();

        // Dummy data
        var reservations = new List<Reservation>
        {
            new Reservation(1, "John", "Doe", 101, DateTime.Now, DateTime.Now.AddDays(7)),
            new Reservation(2, "Jane", "Smith", 102, DateTime.Now.AddDays(-1), DateTime.Now.AddDays(6)),
            new Reservation(3, "Alice", "Johnson", 103, DateTime.Now, DateTime.Now.AddDays(5)),
        };

        // Bind the data
        ReservationsCollectionView.ItemsSource = reservations;
    }
}
