
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace Spine.Unity.AttachmentTools {
	public static class AttachmentRegionExtensions {
		#region GetRegion
		/// <summary>
		/// Tries to get the region (image) of a renderable attachment. If the attachment is not renderable, it returns null.</summary>
		public static AtlasRegion GetRegion (this Attachment attachment) {
			var renderableAttachment = attachment as IHasRendererObject;
			if (renderableAttachment != null)
				return renderableAttachment.RendererObject as AtlasRegion;

			return null;
		}

		/// <summary>Gets the region (image) of a RegionAttachment</summary>
		public static AtlasRegion GetRegion (this RegionAttachment regionAttachment) {
			return regionAttachment.RendererObject as AtlasRegion;
		}

		/// <summary>Gets the region (image) of a MeshAttachment</summary>
		public static AtlasRegion GetRegion (this MeshAttachment meshAttachment) {
			return meshAttachment.RendererObject as AtlasRegion;
		}
		#endregion
		#region SetRegion
		/// <summary>
		/// Tries to set the region (image) of a renderable attachment. If the attachment is not renderable, nothing is applied.</summary>
		public static void SetRegion (this Attachment attachment, AtlasRegion region, bool updateOffset = true) {
			var regionAttachment = attachment as RegionAttachment;
			if (regionAttachment != null)
				regionAttachment.SetRegion(region, updateOffset);

			var meshAttachment = attachment as MeshAttachment;
			if (meshAttachment != null)
				meshAttachment.SetRegion(region, updateOffset);
		}

		/// <summary>Sets the region (image) of a RegionAttachment</summary>
		public static void SetRegion (this RegionAttachment attachment, AtlasRegion region, bool updateOffset = true) {
			if (region == null) throw new System.ArgumentNullException("region");

			// (AtlasAttachmentLoader.cs)
			attachment.RendererObject = region;
			attachment.SetUVs(region.u, region.v, region.u2, region.v2, region.rotate);
			attachment.regionOffsetX = region.offsetX;
			attachment.regionOffsetY = region.offsetY;
			attachment.regionWidth = region.width;
			attachment.regionHeight = region.height;
			attachment.regionOriginalWidth = region.originalWidth;
			attachment.regionOriginalHeight = region.originalHeight;

			if (updateOffset) attachment.UpdateOffset();
		}

		/// <summary>Sets the region (image) of a MeshAttachment</summary>
		public static void SetRegion (this MeshAttachment attachment, AtlasRegion region, bool updateUVs = true) {
			if (region == null) throw new System.ArgumentNullException("region");

			// (AtlasAttachmentLoader.cs)
			attachment.RendererObject = region;
			attachment.RegionU = region.u;
			attachment.RegionV = region.v;
			attachment.RegionU2 = region.u2;
			attachment.RegionV2 = region.v2;
			attachment.RegionRotate = region.rotate;
			attachment.regionOffsetX = region.offsetX;
			attachment.regionOffsetY = region.offsetY;
			attachment.regionWidth = region.width;
			attachment.regionHeight = region.height;
			attachment.regionOriginalWidth = region.originalWidth;
			attachment.regionOriginalHeight = region.originalHeight;

			if (updateUVs) attachment.UpdateUVs();
		}
		#endregion

		#region Runtime RegionAttachments
		/// <summary>
		/// Creates a RegionAttachment based on a sprite. This method creates a real, usable AtlasRegion. That AtlasRegion uses a new AtlasPage with the Material provided./// </summary>
		public static RegionAttachment ToRegionAttachment (this Sprite sprite, Material material, float rotation = 0f) {
			return sprite.ToRegionAttachment(material.ToSpineAtlasPage(), rotation);
		}

		/// <summary>
		/// Creates a RegionAttachment based on a sprite. This method creates a real, usable AtlasRegion. That AtlasRegion uses the AtlasPage provided./// </summary>
		public static RegionAttachment ToRegionAttachment (this Sprite sprite, AtlasPage page, float rotation = 0f) {
			if (sprite == null) throw new System.ArgumentNullException("sprite");
			if (page == null) throw new System.ArgumentNullException("page");
			var region = sprite.ToAtlasRegion(page);
			var unitsPerPixel = 1f / sprite.pixelsPerUnit;
			return region.ToRegionAttachment(sprite.name, unitsPerPixel, rotation);
		}

		/// <summary>
		/// Creates a Spine.AtlasRegion that uses a premultiplied alpha duplicate texture of the Sprite's texture data. Returns a RegionAttachment that uses it. Use this if you plan to use a premultiply alpha shader such as "Spine/Skeleton"</summary>
		public static RegionAttachment ToRegionAttachmentPMAClone (this Sprite sprite, Shader shader, TextureFormat textureFormat = AtlasUtilities.SpineTextureFormat, bool mipmaps = AtlasUtilities.UseMipMaps, Material materialPropertySource = null, float rotation = 0f) {
			if (sprite == null) throw new System.ArgumentNullException("sprite");
			if (shader == null) throw new System.ArgumentNullException("shader");
			var region = sprite.ToAtlasRegionPMAClone(shader, textureFormat, mipmaps, materialPropertySource);
			var unitsPerPixel = 1f / sprite.pixelsPerUnit;
			return region.ToRegionAttachment(sprite.name, unitsPerPixel, rotation);
		}

		public static RegionAttachment ToRegionAttachmentPMAClone (this Sprite sprite, Material materialPropertySource, TextureFormat textureFormat = AtlasUtilities.SpineTextureFormat, bool mipmaps = AtlasUtilities.UseMipMaps, float rotation = 0f) {
			return sprite.ToRegionAttachmentPMAClone(materialPropertySource.shader, textureFormat, mipmaps, materialPropertySource, rotation);
		}

		/// <summary>
		/// Creates a new RegionAttachment from a given AtlasRegion.</summary>
		public static RegionAttachment ToRegionAttachment (this AtlasRegion region, string attachmentName, float scale = 0.01f, float rotation = 0f) {
			if (string.IsNullOrEmpty(attachmentName)) throw new System.ArgumentException("attachmentName can't be null or empty.", "attachmentName");
			if (region == null) throw new System.ArgumentNullException("region");

			// (AtlasAttachmentLoader.cs)
			var attachment = new RegionAttachment(attachmentName);

			attachment.RendererObject = region;
			attachment.SetUVs(region.u, region.v, region.u2, region.v2, region.rotate);
			attachment.regionOffsetX = region.offsetX;
			attachment.regionOffsetY = region.offsetY;
			attachment.regionWidth = region.width;
			attachment.regionHeight = region.height;
			attachment.regionOriginalWidth = region.originalWidth;
			attachment.regionOriginalHeight = region.originalHeight;

			attachment.Path = region.name;
			attachment.scaleX = 1;
			attachment.scaleY = 1;
			attachment.rotation = rotation;

			attachment.r = 1;
			attachment.g = 1;
			attachment.b = 1;
			attachment.a = 1;

			// pass OriginalWidth and OriginalHeight because UpdateOffset uses it in its calculation.
			attachment.width = attachment.regionOriginalWidth * scale;
			attachment.height = attachment.regionOriginalHeight * scale;

			attachment.SetColor(Color.white);
			attachment.UpdateOffset();
			return attachment;
		}

		/// <summary> Sets the scale. Call regionAttachment.UpdateOffset to apply the change.</summary>
		public static void SetScale (this RegionAttachment regionAttachment, Vector2 scale) {
			regionAttachment.scaleX = scale.x;
			regionAttachment.scaleY = scale.y;
		}

		/// <summary> Sets the scale. Call regionAttachment.UpdateOffset to apply the change.</summary>
		public static void SetScale (this RegionAttachment regionAttachment, float x, float y) {
			regionAttachment.scaleX = x;
			regionAttachment.scaleY = y;
		}

		/// <summary> Sets the position offset. Call regionAttachment.UpdateOffset to apply the change.</summary>
		public static void SetPositionOffset (this RegionAttachment regionAttachment, Vector2 offset) {
			regionAttachment.x = offset.x;
			regionAttachment.y = offset.y;
		}

		/// <summary> Sets the position offset. Call regionAttachment.UpdateOffset to apply the change.</summary>
		public static void SetPositionOffset (this RegionAttachment regionAttachment, float x, float y) {
			regionAttachment.x = x;
			regionAttachment.y = y;
		}

		/// <summary> Sets the rotation. Call regionAttachment.UpdateOffset to apply the change.</summary>
		public static void SetRotation (this RegionAttachment regionAttachment, float rotation) {
			regionAttachment.rotation = rotation;
		}
		#endregion
	}
}
