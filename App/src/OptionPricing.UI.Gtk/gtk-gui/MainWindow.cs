
// This file has been generated by the GUI designer. Do not modify.

public partial class MainWindow
{
	private global::Gtk.UIManager UIManager;

	private global::Gtk.Action HerramientasAction;

	private global::Gtk.Action HerramientasAction1;

	private global::Gtk.Action sortAscendingAction;

	private global::Gtk.Action sortAscendingAction1;

	private global::Gtk.Fixed fixed3;

	private global::Gtk.Button btnPruebas;

	protected virtual void Build ()
	{
		global::Stetic.Gui.Initialize (this);
		// Widget MainWindow
		this.UIManager = new global::Gtk.UIManager ();
		global::Gtk.ActionGroup w1 = new global::Gtk.ActionGroup ("Default");
		this.HerramientasAction = new global::Gtk.Action ("HerramientasAction", global::Mono.Unix.Catalog.GetString ("Herramientas"), null, null);
		this.HerramientasAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("Herramientas");
		w1.Add (this.HerramientasAction, null);
		this.HerramientasAction1 = new global::Gtk.Action ("HerramientasAction1", global::Mono.Unix.Catalog.GetString ("Herramientas"), null, null);
		this.HerramientasAction1.ShortLabel = global::Mono.Unix.Catalog.GetString ("Herramientas");
		w1.Add (this.HerramientasAction1, null);
		this.sortAscendingAction = new global::Gtk.Action ("sortAscendingAction", global::Mono.Unix.Catalog.GetString ("_Ascending"), null, "gtk-sort-ascending");
		this.sortAscendingAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("_Ascending");
		w1.Add (this.sortAscendingAction, null);
		this.sortAscendingAction1 = new global::Gtk.Action ("sortAscendingAction1", global::Mono.Unix.Catalog.GetString ("Pruebas"), null, "gtk-sort-ascending");
		this.sortAscendingAction1.ShortLabel = global::Mono.Unix.Catalog.GetString ("Pruebas");
		w1.Add (this.sortAscendingAction1, null);
		this.UIManager.InsertActionGroup (w1, 0);
		this.AddAccelGroup (this.UIManager.AccelGroup);
		this.Name = "MainWindow";
		this.Title = global::Mono.Unix.Catalog.GetString ("MainWindow");
		this.WindowPosition = ((global::Gtk.WindowPosition)(4));
		// Container child MainWindow.Gtk.Container+ContainerChild
		this.fixed3 = new global::Gtk.Fixed ();
		this.fixed3.Name = "fixed3";
		this.fixed3.HasWindow = false;
		// Container child fixed3.Gtk.Fixed+FixedChild
		this.btnPruebas = new global::Gtk.Button ();
		this.btnPruebas.CanFocus = true;
		this.btnPruebas.Name = "btnPruebas";
		this.btnPruebas.UseUnderline = true;
		this.btnPruebas.Label = global::Mono.Unix.Catalog.GetString ("Pruebas");
		this.fixed3.Add (this.btnPruebas);
		global::Gtk.Fixed.FixedChild w2 = ((global::Gtk.Fixed.FixedChild)(this.fixed3[this.btnPruebas]));
		w2.X = 15;
		w2.Y = 14;
		this.Add (this.fixed3);
		if ((this.Child != null)) {
			this.Child.ShowAll ();
		}
		this.DefaultWidth = 760;
		this.DefaultHeight = 365;
		this.Show ();
		this.DeleteEvent += new global::Gtk.DeleteEventHandler (this.OnDeleteEvent);
		this.btnPruebas.Clicked += new global::System.EventHandler (this.OnBtnPruebasClicked);
	}
}