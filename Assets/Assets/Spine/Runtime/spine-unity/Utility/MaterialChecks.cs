
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR

namespace Spine.Unity {
	/// <summary>Utility class providing methods to check material settings for incorrect combinations.</summary>
	public class MaterialChecks {

		static readonly int STRAIGHT_ALPHA_PARAM_ID = Shader.PropertyToID("_StraightAlphaInput");
		static readonly string ALPHAPREMULTIPLY_ON_KEYWORD = "_ALPHAPREMULTIPLY_ON";

		public static readonly string kPMANotSupportedLinearMessage =
			"Warning: Premultiply-alpha atlas textures not supported in Linear color space!\n\nPlease\n"
			+ "a) re-export atlas as straight alpha texture with 'premultiply alpha' unchecked\n"
			+ "   (if you have already done this, please set the 'Straight Alpha Texture' Material parameter to 'true') or\n"
			+ "b) switch to Gamma color space via\nProject Settings - Player - Other Settings - Color Space.\n";
		public static readonly string kZSpacingRequiredMessage =
			"Warning: Z Spacing required on selected shader! Otherwise you will receive incorrect results.\n\nPlease\n"
			+ "1) make sure at least minimal 'Z Spacing' is set at the SkeletonRenderer/SkeletonAnimation component under 'Advanced' and\n"
			+ "2) ensure that the skeleton has overlapping parts on different Z depth. You can adjust this in Spine via draw order.\n";
		public static readonly string kZSpacingRecommendedMessage =
			"Warning: Z Spacing recommended on selected shader configuration!\n\nPlease\n"
			+ "1) make sure at least minimal 'Z Spacing' is set at the SkeletonRenderer/SkeletonAnimation component under 'Advanced' and\n"
			+ "2) ensure that the skeleton has overlapping parts on different Z depth. You can adjust this in Spine via draw order.\n";

		public static bool IsMaterialSetupProblematic (SkeletonRenderer renderer, ref string errorMessage) {
			var materials = renderer.GetComponent<Renderer>().sharedMaterials;
			bool isProblematic = false;
			foreach (var mat in materials) {
				if (mat == null) continue;
				isProblematic |= IsMaterialSetupProblematic(mat, ref errorMessage);
				if (renderer.zSpacing == 0) {
					isProblematic |= IsZSpacingRequired(mat, ref errorMessage);
				}
			}
			return isProblematic;
		}

		public static bool IsMaterialSetupProblematic(Material material, ref string errorMessage) {
			return !IsColorSpaceSupported(material, ref errorMessage);
		}

		public static bool IsZSpacingRequired(Material material, ref string errorMessage) {
			bool hasForwardAddPass = material.FindPass("FORWARD_DELTA") >= 0;
			if (hasForwardAddPass) {
				errorMessage += kZSpacingRequiredMessage;
				return true;
			}
			bool zWrite = material.HasProperty("_ZWrite") && material.GetFloat("_ZWrite") > 0.0f;
			if (zWrite) {
				errorMessage += kZSpacingRecommendedMessage;
				return true;
			}
			return false;
		}

		public static bool IsColorSpaceSupported (Material material, ref string errorMessage) {
			if (QualitySettings.activeColorSpace == ColorSpace.Linear) {
				if (IsPMAMaterial(material)) {
					errorMessage += kPMANotSupportedLinearMessage;
					return false;
				}
			}
			return true;
		}


		public static bool UsesSpineShader (Material material) {
			return material.shader.name.Contains("Spine/");
		}

		public static bool IsTextureSetupProblematic (Material material, ColorSpace colorSpace,
													bool sRGBTexture, bool mipmapEnabled, bool alphaIsTransparency,
													string texturePath, string materialPath,
													ref string errorMessage) {
			if (material == null || !UsesSpineShader(material)) {
				return false;
			}

			bool isProblematic = false;
			if (IsPMAMaterial(material)) {
				// 'sRGBTexture = true' generates incorrectly weighted mipmaps at PMA textures,
				// causing white borders due to undesired custom weighting.
				if (sRGBTexture && mipmapEnabled && colorSpace == ColorSpace.Gamma) {
					errorMessage += string.Format("`{0}` : Problematic Texture Settings found: When enabling `Generate Mip Maps` in Gamma color space, it is recommended to disable `sRGB (Color Texture)` on `Premultiply alpha` textures. Otherwise you will receive white border artifacts on an atlas exported with default `Premultiply alpha` settings.\n(You can disable this warning in `Edit - Preferences - Spine`)\n", texturePath);
					isProblematic = true;
				}
				if (alphaIsTransparency) {
					string materialName = System.IO.Path.GetFileName(materialPath);
					errorMessage += string.Format("`{0}` and material `{1}` : Problematic Texture / Material Settings found: It is recommended to disable `Alpha Is Transparency` on `Premultiply alpha` textures.\nAssuming `Premultiply alpha` texture because `Straight Alpha Texture` is disabled at material). (You can disable this warning in `Edit - Preferences - Spine`)\n", texturePath, materialName);
					isProblematic = true;
				}
			}
			else { // straight alpha texture
				if (!alphaIsTransparency) {
					string materialName = System.IO.Path.GetFileName(materialPath);
					errorMessage += string.Format("`{0}` and material `{1}` : Incorrect Texture / Material Settings found: It is strongly recommended to enable `Alpha Is Transparency` on `Straight alpha` textures.\nAssuming `Straight alpha` texture because `Straight Alpha Texture` is enabled at material). (You can disable this warning in `Edit - Preferences - Spine`)\n", texturePath, materialName);
					isProblematic = true;
				}
			}
			return isProblematic;
		}

		static bool IsPMAMaterial (Material material) {
			return (material.HasProperty(STRAIGHT_ALPHA_PARAM_ID) && material.GetInt(STRAIGHT_ALPHA_PARAM_ID) == 0) ||
					material.IsKeywordEnabled(ALPHAPREMULTIPLY_ON_KEYWORD);
		}
	}
}

#endif // UNITY_EDITOR
