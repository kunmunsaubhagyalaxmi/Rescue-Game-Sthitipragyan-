
using System;

namespace Spine {

	/// <summary>
	/// An AttachmentLoader that configures attachments using texture regions from an Atlas.
	/// See <a href='http://esotericsoftware.com/spine-loading-skeleton-data#JSON-and-binary-data'>Loading Skeleton Data</a> in the Spine Runtimes Guide.
	/// </summary>
	public class AtlasAttachmentLoader : AttachmentLoader {
		private Atlas[] atlasArray;

		public AtlasAttachmentLoader (params Atlas[] atlasArray) {
			if (atlasArray == null) throw new ArgumentNullException("atlas array cannot be null.");
			this.atlasArray = atlasArray;
		}

		public RegionAttachment NewRegionAttachment (Skin skin, string name, string path) {
			AtlasRegion region = FindRegion(path);
			if (region == null) throw new ArgumentException(string.Format("Region not found in atlas: {0} (region attachment: {1})", path, name));
			RegionAttachment attachment = new RegionAttachment(name);
			attachment.RendererObject = region;
			attachment.SetUVs(region.u, region.v, region.u2, region.v2, region.rotate);
			attachment.regionOffsetX = region.offsetX;
			attachment.regionOffsetY = region.offsetY;
			attachment.regionWidth = region.width;
			attachment.regionHeight = region.height;
			attachment.regionOriginalWidth = region.originalWidth;
			attachment.regionOriginalHeight = region.originalHeight;
			return attachment;
		}

		public MeshAttachment NewMeshAttachment (Skin skin, string name, string path) {
			AtlasRegion region = FindRegion(path);
			if (region == null) throw new ArgumentException(string.Format("Region not found in atlas: {0} (region attachment: {1})", path, name));
			MeshAttachment attachment = new MeshAttachment(name);
			attachment.RendererObject = region;
			attachment.RegionU = region.u;
			attachment.RegionV = region.v;
			attachment.RegionU2 = region.u2;
			attachment.RegionV2 = region.v2;
			attachment.RegionRotate = region.rotate;
			attachment.RegionDegrees = region.degrees;
			attachment.regionOffsetX = region.offsetX;
			attachment.regionOffsetY = region.offsetY;
			attachment.regionWidth = region.width;
			attachment.regionHeight = region.height;
			attachment.regionOriginalWidth = region.originalWidth;
			attachment.regionOriginalHeight = region.originalHeight;
			return attachment;
		}

		public BoundingBoxAttachment NewBoundingBoxAttachment (Skin skin, string name) {
			return new BoundingBoxAttachment(name);
		}

		public PathAttachment NewPathAttachment (Skin skin, string name) {
			return new PathAttachment(name);
		}

		public PointAttachment NewPointAttachment (Skin skin, string name) {
			return new PointAttachment(name);
		}

		public ClippingAttachment NewClippingAttachment(Skin skin, string name) {
			return new ClippingAttachment(name);
		}

		public AtlasRegion FindRegion (string name) {
			AtlasRegion region;

			for (int i = 0; i < atlasArray.Length; i++) {
				region = atlasArray[i].FindRegion(name);
				if (region != null)
					return region;
			}

			return null;
		}
	}
}
