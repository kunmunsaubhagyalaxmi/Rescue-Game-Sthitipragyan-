
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace Spine.Unity.AttachmentTools {

	public static class SkinUtilities {

		#region Skeleton Skin Extensions
		/// <summary>
		/// Convenience method for duplicating a skeleton's current active skin so changes to it will not affect other skeleton instances. .</summary>
		public static Skin UnshareSkin (this Skeleton skeleton, bool includeDefaultSkin, bool unshareAttachments, AnimationState state = null) {
			// 1. Copy the current skin and set the skeleton's skin to the new one.
			var newSkin = skeleton.GetClonedSkin("cloned skin", includeDefaultSkin, unshareAttachments, true);
			skeleton.SetSkin(newSkin);

			// 2. Apply correct attachments: skeleton.SetToSetupPose + animationState.Apply
			if (state != null) {
				skeleton.SetToSetupPose();
				state.Apply(skeleton);
			}

			// 3. Return unshared skin.
			return newSkin;
		}

		public static Skin GetClonedSkin (this Skeleton skeleton, string newSkinName, bool includeDefaultSkin = false, bool cloneAttachments = false, bool cloneMeshesAsLinked = true) {
			var newSkin = new Skin(newSkinName); // may have null name. Harmless.
			var defaultSkin = skeleton.data.DefaultSkin;
			var activeSkin = skeleton.skin;

			if (includeDefaultSkin)
				defaultSkin.CopyTo(newSkin, true, cloneAttachments, cloneMeshesAsLinked);

			if (activeSkin != null)
				activeSkin.CopyTo(newSkin, true, cloneAttachments, cloneMeshesAsLinked);

			return newSkin;
		}
		#endregion

		/// <summary>
		/// Gets a shallow copy of the skin. The cloned skin's attachments are shared with the original skin.</summary>
		public static Skin GetClone (this Skin original) {
			var newSkin = new Skin(original.name + " clone");
			var newSkinAttachments = newSkin.Attachments;

			foreach (var a in original.Attachments)
				newSkinAttachments[a.Key] = a.Value;

			return newSkin;
		}

		/// <summary>Adds an attachment to the skin for the specified slot index and name. If the name already exists for the slot, the previous value is replaced.</summary>
		public static void SetAttachment (this Skin skin, string slotName, string keyName, Attachment attachment, Skeleton skeleton) {
			int slotIndex = skeleton.FindSlotIndex(slotName);
			if (skeleton == null) throw new System.ArgumentNullException("skeleton", "skeleton cannot be null.");
			if (slotIndex == -1) throw new System.ArgumentException(string.Format("Slot '{0}' does not exist in skeleton.", slotName), "slotName");
			skin.SetAttachment(slotIndex, keyName, attachment);
		}

		/// <summary>Adds skin items from another skin. For items that already exist, the previous values are replaced.</summary>
		public static void AddAttachments (this Skin skin, Skin otherSkin) {
			if (otherSkin == null) return;
			otherSkin.CopyTo(skin, true, false);
		}

		/// <summary>Gets an attachment from the skin for the specified slot index and name.</summary>
		public static Attachment GetAttachment (this Skin skin, string slotName, string keyName, Skeleton skeleton) {
			int slotIndex = skeleton.FindSlotIndex(slotName);
			if (skeleton == null) throw new System.ArgumentNullException("skeleton", "skeleton cannot be null.");
			if (slotIndex == -1) throw new System.ArgumentException(string.Format("Slot '{0}' does not exist in skeleton.", slotName), "slotName");
			return skin.GetAttachment(slotIndex, keyName);
		}

		/// <summary>Adds an attachment to the skin for the specified slot index and name. If the name already exists for the slot, the previous value is replaced.</summary>
		public static void SetAttachment (this Skin skin, int slotIndex, string keyName, Attachment attachment) {
			skin.SetAttachment(slotIndex, keyName, attachment);
		}

		public static void RemoveAttachment (this Skin skin, string slotName, string keyName, SkeletonData skeletonData) {
			int slotIndex = skeletonData.FindSlotIndex(slotName);
			if (skeletonData == null) throw new System.ArgumentNullException("skeletonData", "skeletonData cannot be null.");
			if (slotIndex == -1) throw new System.ArgumentException(string.Format("Slot '{0}' does not exist in skeleton.", slotName), "slotName");
			skin.RemoveAttachment(slotIndex, keyName);
		}

		public static void Clear (this Skin skin) {
			skin.Attachments.Clear();
		}

		//[System.Obsolete]
		public static void Append (this Skin destination, Skin source) {
			source.CopyTo(destination, true, false);
		}

		public static void CopyTo (this Skin source, Skin destination, bool overwrite, bool cloneAttachments, bool cloneMeshesAsLinked = true) {
			var sourceAttachments = source.Attachments;
			var destinationAttachments = destination.Attachments;

			if (cloneAttachments) {
				if (overwrite) {
					foreach (var e in sourceAttachments)
						destinationAttachments[e.Key] = e.Value.GetCopy(cloneMeshesAsLinked);
				} else {
					foreach (var e in sourceAttachments) {
						if (destinationAttachments.ContainsKey(e.Key)) continue;
						destinationAttachments.Add(e.Key, e.Value.GetCopy(cloneMeshesAsLinked));
					}
				}
			} else {
				if (overwrite) {
					foreach (var e in sourceAttachments)
						destinationAttachments[e.Key] = e.Value;
				} else {
					foreach (var e in sourceAttachments) {
						if (destinationAttachments.ContainsKey(e.Key)) continue;
						destinationAttachments.Add(e.Key, e.Value);
					}
				}
			}
		}


	}

}
