namespace Editor;

partial class SceneNode : GameObjectNode
{
	public SceneNode( Scene scene ) : base( scene )
	{

	}

	public override bool HasChildren => Value.Children.Where( x => x.ShouldShowInHierarchy() ).Any();

	protected override void BuildChildren() => SetChildren( Value.Children.Where( x => x.ShouldShowInHierarchy() ), x => new GameObjectNode( x ) );
	protected override bool HasDescendant( object obj ) => obj is GameObject go && Value.IsDescendant( go );

	public override void OnPaint( VirtualWidget item )
	{
		var r = item.Rect;
		Paint.SetPen( Theme.TextControl.WithAlpha( 0.4f ) );

		r.Left += 4;
		Paint.DrawIcon( r, "perm_media", 14, TextFlag.LeftCenter );

		r.Left += 22;
		Paint.SetDefaultFont();
		Paint.DrawText( r, $"{Value.Name}", TextFlag.LeftCenter );
	}

	public override int ValueHash
	{
		get
		{
			if ( Value?.Children is null )
				return 0;

			HashCode hc = new HashCode();

			foreach ( var val in Value.Children )
			{
				if ( !val.ShouldShowInHierarchy() ) continue;
				hc.Add( val );
			}

			return hc.ToHashCode();
		}
	}

	public override bool OnContextMenu()
	{
		var m = new ContextMenu( TreeView );

		m.AddSeparator();

		GameObjectNode.AddGameObjectMenuItems( m, this );

		m.OpenAtCursor( false );

		return true;
	}

	public override bool OnDragStart()
	{
		return false;
	}
}

