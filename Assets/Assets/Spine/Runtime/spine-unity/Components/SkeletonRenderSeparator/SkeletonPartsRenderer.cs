
using UnityEngine;

namespace Spine.Unity {
	[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
	public class SkeletonPartsRenderer : MonoBehaviour {

		#region Properties
		MeshGenerator meshGenerator;
		public MeshGenerator MeshGenerator {
			get {
				LazyIntialize();
				return meshGenerator;
			}
		}

		MeshRenderer meshRenderer;
		public MeshRenderer MeshRenderer {
			get {
				LazyIntialize();
				return meshRenderer;
			}
		}

		MeshFilter meshFilter;
		public MeshFilter MeshFilter {
			get {
				LazyIntialize();
				return meshFilter;
			}
		}
		#endregion

		MeshRendererBuffers buffers;
		SkeletonRendererInstruction currentInstructions = new SkeletonRendererInstruction();


		void LazyIntialize () {
			if (buffers == null) {
				buffers = new MeshRendererBuffers();
				buffers.Initialize();

				if (meshGenerator != null) return;
				meshGenerator = new MeshGenerator();
				meshFilter = GetComponent<MeshFilter>();
				meshRenderer = GetComponent<MeshRenderer>();
				currentInstructions.Clear();
			}
		}

		public void ClearMesh () {
			LazyIntialize();
			meshFilter.sharedMesh = null;
		}

		public void RenderParts (ExposedList<SubmeshInstruction> instructions, int startSubmesh, int endSubmesh) {
			LazyIntialize();

			// STEP 1: Create instruction
			var smartMesh = buffers.GetNextMesh();
			currentInstructions.SetWithSubset(instructions, startSubmesh, endSubmesh);
			bool updateTriangles = SkeletonRendererInstruction.GeometryNotEqual(currentInstructions, smartMesh.instructionUsed);

			// STEP 2: Generate mesh buffers.
			var currentInstructionsSubmeshesItems = currentInstructions.submeshInstructions.Items;
			meshGenerator.Begin();
			if (currentInstructions.hasActiveClipping) {
				for (int i = 0; i < currentInstructions.submeshInstructions.Count; i++)
					meshGenerator.AddSubmesh(currentInstructionsSubmeshesItems[i], updateTriangles);
			} else {
				meshGenerator.BuildMeshWithArrays(currentInstructions, updateTriangles);
			}

			buffers.UpdateSharedMaterials(currentInstructions.submeshInstructions);

			// STEP 3: modify mesh.
			var mesh = smartMesh.mesh;

			if (meshGenerator.VertexCount <= 0) { // Clear an empty mesh
				updateTriangles = false;
				mesh.Clear();
			} else {
				meshGenerator.FillVertexData(mesh);
				if (updateTriangles) {
					meshGenerator.FillTriangles(mesh);
					meshRenderer.sharedMaterials = buffers.GetUpdatedSharedMaterialsArray();
				} else if (buffers.MaterialsChangedInLastUpdate()) {
					meshRenderer.sharedMaterials = buffers.GetUpdatedSharedMaterialsArray();
				}
				meshGenerator.FillLateVertexData(mesh);
			}

			meshFilter.sharedMesh = mesh;
			smartMesh.instructionUsed.Set(currentInstructions);
		}

		public void SetPropertyBlock (MaterialPropertyBlock block) {
			LazyIntialize();
			meshRenderer.SetPropertyBlock(block);
		}

		public static SkeletonPartsRenderer NewPartsRendererGameObject (Transform parent, string name, int sortingOrder = 0) {
			var go = new GameObject(name, typeof(MeshFilter), typeof(MeshRenderer));
			go.transform.SetParent(parent, false);
			var returnComponent = go.AddComponent<SkeletonPartsRenderer>();
			returnComponent.MeshRenderer.sortingOrder = sortingOrder;

			return returnComponent;
		}
	}
}
