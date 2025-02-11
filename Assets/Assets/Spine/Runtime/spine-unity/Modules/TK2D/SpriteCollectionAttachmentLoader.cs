
#if SPINE_TK2D
using System;
using UnityEngine;
using Spine;

// MITCH: handle TPackerCW flip mode (probably not swap uv horizontaly)
namespace Spine.Unity.TK2D {
	public class SpriteCollectionAttachmentLoader : AttachmentLoader {
		private tk2dSpriteCollectionData sprites;
		private float u, v, u2, v2;
		private bool regionRotated;
		private float regionOriginalWidth, regionOriginalHeight;
		private float regionWidth, regionHeight;
		private float regionOffsetX, regionOffsetY;
		private Material material;

		public SpriteCollectionAttachmentLoader (tk2dSpriteCollectionData sprites) {
			if (sprites == null)
				throw new ArgumentNullException("sprites cannot be null.");
			this.sprites = sprites;
		}

		private void ProcessSpriteDefinition (String name) {
			// Strip folder names.
			int index = name.LastIndexOfAny(new char[] {'/', '\\'});
			if (index != -1)
				name = name.Substring(index + 1);

			tk2dSpriteDefinition def = sprites.inst.GetSpriteDefinition(name);

			if (def == null) {
				Debug.Log("Sprite not found in atlas: " + name, sprites);
				throw new Exception("Sprite not found in atlas: " + name);
			}
			if (def.complexGeometry)
				throw new NotImplementedException("Complex geometry is not supported: " + name);
			if (def.flipped == tk2dSpriteDefinition.FlipMode.TPackerCW)
				throw new NotImplementedException("Only 2D Toolkit atlases are supported: " + name);

			Vector2 minTexCoords = Vector2.one, maxTexCoords = Vector2.zero;
			for (int i = 0; i < def.uvs.Length; ++i) {
				Vector2 uv = def.uvs[i];
				minTexCoords = Vector2.Min(minTexCoords, uv);
				maxTexCoords = Vector2.Max(maxTexCoords, uv);
			}
			regionRotated = def.flipped == tk2dSpriteDefinition.FlipMode.Tk2d;
			if (regionRotated) {
				float temp = minTexCoords.x;
				minTexCoords.x = maxTexCoords.x;
				maxTexCoords.x = temp;
			}
			u = minTexCoords.x;
			v = maxTexCoords.y;
			u2 = maxTexCoords.x;
			v2 = minTexCoords.y;

			regionOriginalWidth = (int)(def.untrimmedBoundsData[1].x / def.texelSize.x);
			regionOriginalHeight = (int)(def.untrimmedBoundsData[1].y / def.texelSize.y);

			regionWidth = (int)(def.boundsData[1].x / def.texelSize.x);
			regionHeight = (int)(def.boundsData[1].y / def.texelSize.y);

			float x0 = def.untrimmedBoundsData[0].x - def.untrimmedBoundsData[1].x / 2;
			float x1 = def.boundsData[0].x - def.boundsData[1].x / 2;
			regionOffsetX = (int)((x1 - x0) / def.texelSize.x);

			float y0 = def.untrimmedBoundsData[0].y - def.untrimmedBoundsData[1].y / 2;
			float y1 = def.boundsData[0].y - def.boundsData[1].y / 2;
			regionOffsetY = (int)((y1 - y0) / def.texelSize.y);

			material = def.materialInst;
		}

		public RegionAttachment NewRegionAttachment (Skin skin, String name, String path) {
			ProcessSpriteDefinition(path);

			RegionAttachment region = new RegionAttachment(name);
			region.Path = path;
			region.RendererObject = material;
			region.SetUVs(u, v, u2, v2, regionRotated);
			region.RegionOriginalWidth = regionOriginalWidth;
			region.RegionOriginalHeight = regionOriginalHeight;
			region.RegionWidth = regionWidth;
			region.RegionHeight = regionHeight;
			region.RegionOffsetX = regionOffsetX;
			region.RegionOffsetY = regionOffsetY;
			return region;
		}

		public MeshAttachment NewMeshAttachment (Skin skin, String name, String path) {
			ProcessSpriteDefinition(path);

			MeshAttachment mesh = new MeshAttachment(name);
			mesh.Path = path;
			mesh.RendererObject = material;
			mesh.RegionU = u;
			mesh.RegionV = v;
			mesh.RegionU2 = u2;
			mesh.RegionV2 = v2;
			mesh.RegionRotate = regionRotated;
			mesh.RegionOriginalWidth = regionOriginalWidth;
			mesh.RegionOriginalHeight = regionOriginalHeight;
			mesh.RegionWidth = regionWidth;
			mesh.RegionHeight = regionHeight;
			mesh.RegionOffsetX = regionOffsetX;
			mesh.RegionOffsetY = regionOffsetY;
			return mesh;
		}

		public BoundingBoxAttachment NewBoundingBoxAttachment (Skin skin, String name) {
			return new BoundingBoxAttachment(name);
		}

		public PathAttachment NewPathAttachment (Skin skin, string name) {
			return new PathAttachment(name);
		}

		public PointAttachment NewPointAttachment (Skin skin, string name) {
			return new PointAttachment(name);
		}

		public ClippingAttachment NewClippingAttachment (Skin skin, string name) {
			return new ClippingAttachment(name);
		}
	}
}
#endif
