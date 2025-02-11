
namespace Spine {

	///<summary>The interface for items updated by <see cref="Skeleton.UpdateWorldTransform()"/>.</summary>
	public interface IUpdatable {
		void Update ();

		///<summary>Returns false when this item has not been updated because a skin is required and the <see cref="Skeleton.Skin">active
		/// skin</see> does not contain this item.</summary>
		/// <seealso cref="Skin.Bones"/>
		/// <seealso cref="Skin.Constraints"/>
		bool Active { get; }
	}
}
