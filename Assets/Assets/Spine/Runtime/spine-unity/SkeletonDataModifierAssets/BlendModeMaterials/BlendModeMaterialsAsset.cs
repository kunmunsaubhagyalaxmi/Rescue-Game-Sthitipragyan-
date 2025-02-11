
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Spine;
using Spine.Unity;

namespace Spine.Unity {
	[CreateAssetMenu(menuName = "Spine/SkeletonData Modifiers/Blend Mode Materials", order = 200)]
	public class BlendModeMaterialsAsset : SkeletonDataModifierAsset {
		public Material multiplyMaterialTemplate;
		public Material screenMaterialTemplate;
		public Material additiveMaterialTemplate;

		public bool applyAdditiveMaterial = true;

		public override void Apply (SkeletonData skeletonData) {
			ApplyMaterials(skeletonData, multiplyMaterialTemplate, screenMaterialTemplate, additiveMaterialTemplate, applyAdditiveMaterial);
		}

		public static void ApplyMaterials (SkeletonData skeletonData, Material multiplyTemplate, Material screenTemplate, Material additiveTemplate, bool includeAdditiveSlots) {
			if (skeletonData == null) throw new ArgumentNullException("skeletonData");

			using (var materialCache = new AtlasMaterialCache()) {
				var entryBuffer = new List<Skin.SkinEntry>();
				var slotsItems = skeletonData.Slots.Items;
				for (int slotIndex = 0, slotCount = skeletonData.Slots.Count; slotIndex < slotCount; slotIndex++) {
					var slot = slotsItems[slotIndex];
					if (slot.blendMode == BlendMode.Normal) continue;
					if (!includeAdditiveSlots && slot.blendMode == BlendMode.Additive) continue;

					entryBuffer.Clear();
					foreach (var skin in skeletonData.Skins)
						skin.GetAttachments(slotIndex, entryBuffer);

					Material templateMaterial = null;
					switch (slot.blendMode) {
						case BlendMode.Multiply:
							templateMaterial = multiplyTemplate;
							break;
						case BlendMode.Screen:
							templateMaterial = screenTemplate;
							break;
						case BlendMode.Additive:
							templateMaterial = additiveTemplate;
							break;
					}
					if (templateMaterial == null) continue;

					foreach (var entry in entryBuffer) {
						var renderableAttachment = entry.Attachment as IHasRendererObject;
						if (renderableAttachment != null) {
							renderableAttachment.RendererObject = materialCache.CloneAtlasRegionWithMaterial((AtlasRegion)renderableAttachment.RendererObject, templateMaterial);
						}
					}
				}

			}
			//attachmentBuffer.Clear();
		}

		class AtlasMaterialCache : IDisposable {
			readonly Dictionary<KeyValuePair<AtlasPage, Material>, AtlasPage> cache = new Dictionary<KeyValuePair<AtlasPage, Material>, AtlasPage>();

			/// <summary>Creates a clone of an AtlasRegion that uses different Material settings, while retaining the original texture.</summary>
			public AtlasRegion CloneAtlasRegionWithMaterial (AtlasRegion originalRegion, Material materialTemplate) {
				var newRegion = originalRegion.Clone();
				newRegion.page = GetAtlasPageWithMaterial(originalRegion.page, materialTemplate);
				return newRegion;
			}

			AtlasPage GetAtlasPageWithMaterial (AtlasPage originalPage, Material materialTemplate) {
				if (originalPage == null) throw new ArgumentNullException("originalPage");

				AtlasPage newPage = null;
				var key = new KeyValuePair<AtlasPage, Material>(originalPage, materialTemplate);
				cache.TryGetValue(key, out newPage);

				if (newPage == null) {
					newPage = originalPage.Clone();
					var originalMaterial = originalPage.rendererObject as Material;
					newPage.rendererObject = new Material(materialTemplate) {
						name = originalMaterial.name + " " + materialTemplate.name,
						mainTexture = originalMaterial.mainTexture
					};
					cache.Add(key, newPage);
				}

				return newPage;
			}

			public void Dispose () {
				cache.Clear();
			}
		}

	}

}
