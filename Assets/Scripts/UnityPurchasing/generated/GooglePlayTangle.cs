// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("EZKck6MRkpmREZKSkzNbLM7UbcwMsvhKfSx8uabyXUJaNCJpNPAhjHqJAvX0JG0ggIMXnjyH/sqjq4hTtJb/vr+Xru2zYEQuf9U6YvwY78FyAOpdKbqtrMR0GlAntXfp9EjbbbZKwkS9WiKmTzPM7qzqFlxHwQMeqeBLZj8WiSmvID0TQbwWoY06zjSdSsszW1ZRvoPyMEYfgJCYyH52jKMRkrGjnpWauRXbFWSekpKSlpOQysTISxxCm0aoPT6IoEPeamyR1xkxBNJw6k6/oAoPXQszRQgdP+5nHuDzXAic9GMpFrqFCfl1pz1KYTF2ME3XmfPux5nXH5KPGC36eS+3RtFljPlS1WdB+ekxSp2ycHuIJBEAtFNkUGOkmKUqZpGQkpOS");
        private static int[] order = new int[] { 1,9,10,12,7,8,7,8,9,9,12,12,13,13,14 };
        private static int key = 147;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
