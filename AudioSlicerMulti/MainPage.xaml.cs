using System.Collections.ObjectModel;

namespace AudioSlicerMulti
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public ObservableCollection<AudiobookTask> Tasks { get; set; }
        public ContentView DropZone { get; set; }

        public MainPage()
        {

            InitializeComponent();
            Tasks = new ObservableCollection<AudiobookTask>()
            {
                new AudiobookTask("test", "est", "awetg")
            };
            DropZone = new ContentView()
            {
                BackgroundColor = Colors.Black,
                WidthRequest = 200,
                HeightRequest = 100
            };
            var dragGestureRecognizer = new DragGestureRecognizer();
            dragGestureRecognizer.DropCompleted += OnDrop;
            DropZone.GestureRecognizers.Add(dragGestureRecognizer);
            BindingContext = this;
        }

        private void OnDrop(object sender, DropCompletedEventArgs e)
        {
            Console.WriteLine("it worked");
        }

        private void OnCounterClicked(object sender, EventArgs e)
        {
            count++;

            if (count == 1)
                CounterBtn.Text = $"Clicked {count} time";
            else
                CounterBtn.Text = $"Clicked {count} times";

            SemanticScreenReader.Announce(CounterBtn.Text);
        }
    }
}