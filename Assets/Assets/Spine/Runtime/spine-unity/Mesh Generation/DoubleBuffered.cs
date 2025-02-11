
namespace Spine.Unity {
	public class DoubleBuffered<T> where T : new() {
		readonly T a = new T();
		readonly T b = new T();
		bool usingA;

		public T GetCurrent () {
			return usingA ? a : b;
		}

		public T GetNext () {
			usingA = !usingA;
			return usingA ? a : b;
		}
	}
}
