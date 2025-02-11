
using System;
using UnityEngine;

namespace Spine.Unity {

	public class RegionlessAttachmentLoader : AttachmentLoader {

		static AtlasRegion emptyRegion;
		static AtlasRegion EmptyRegion {
			get {
				if (emptyRegion == null) {
					emptyRegion = new AtlasRegion {
						name = "Empty AtlasRegion",
						page = new AtlasPage {
							name = "Empty AtlasPage",
							rendererObject = new Material(Shader.Find("Spine/Special/HiddenPass")) { name = "NoRender Material" }
						}
					};
				}
				return emptyRegion;
			}
		}

		public RegionAttachment NewRegionAttachment (Skin skin, string name, string path) {
			RegionAttachment attachment = new RegionAttachment(name) {
				RendererObject = EmptyRegion
			};
			return attachment;
		}

		public MeshAttachment NewMeshAttachment (Skin skin, string name, string path) {
			MeshAttachment attachment = new MeshAttachment(name) {
				RendererObject = EmptyRegion
			};
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

		public ClippingAttachment NewClippingAttachment (Skin skin, string name) {
			return new ClippingAttachment(name);
		}
	}
}
