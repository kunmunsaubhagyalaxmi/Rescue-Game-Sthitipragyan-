
namespace Spine {
	public interface AttachmentLoader {
		/// <return>May be null to not load any attachment.</return>
		RegionAttachment NewRegionAttachment (Skin skin, string name, string path);

		/// <return>May be null to not load any attachment.</return>
		MeshAttachment NewMeshAttachment (Skin skin, string name, string path);

		/// <return>May be null to not load any attachment.</return>
		BoundingBoxAttachment NewBoundingBoxAttachment (Skin skin, string name);

		/// <returns>May be null to not load any attachment</returns>
		PathAttachment NewPathAttachment (Skin skin, string name);

		PointAttachment NewPointAttachment (Skin skin, string name);

		ClippingAttachment NewClippingAttachment (Skin skin, string name);
	}
}
