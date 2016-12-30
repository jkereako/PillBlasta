public static class Utility {

  // Implementation of the Fisher-Yates shuffle algorithm.
  // https://en.wikipedia.org/wiki/Fisher–Yates_shuffle
  public static T[] Shuffle<T>(T[] array, int seed) {
    System.Random randomGenerator = new System.Random(seed);
    for (int i = 0; i < array.Length - 1; i++) {
      int randomIndex = randomGenerator.Next(i, array.Length);
      T temp = array[i];
      array[i] = array[randomIndex];
      array[randomIndex] = temp;
    }

    return array;
  }
}
