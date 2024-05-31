using System.Diagnostics;
using System.Text;

const string target = "سلام، حالت خوبه؟";
List<Chromosome> population = new List<Chromosome>();

void CreatePopulation()
{
    Random random = new();
    for (int i = 0; i < 10; i++)
    {
        string str = new string(target.OrderBy(character => (random.Next(2) % 2) == 0).ToArray());
        int fitness = Fitness(str);
        population.Add(new() { Str = str, Fitness = fitness });
    }
}

int Fitness(string str)
{
    int fitness = 0;
    for (int i = 0; i < target.Length; i++)
        if (str[i] == target[i])
            fitness++;
    return fitness;
}

void CrossOver()
{
    population = population.OrderByDescending(chromosome => chromosome.Fitness).ToList();
    var random1 = Random.Shared.Next(0, 5);
    var random2 = Random.Shared.Next(0, population.Count / 2);
    Chromosome firstChromosomeParent = population[random1];
    Chromosome secondChromosomeParent = population[random2];

    int length = target.Length;
    int random = Random.Shared.Next(0, length);

    string str = string.Concat(firstChromosomeParent.Str.Substring(0, random), secondChromosomeParent.Str.Substring(random));
    int fitness = Fitness(str);
    var firstChromosomeChild = new Chromosome
    {
        Str = str,
        Fitness = Fitness(str)
    };

    Mutate(ref firstChromosomeChild);

    if (!population.Contains(firstChromosomeChild))
    {
        Console.WriteLine("{0} - {1}", ConvertToRTL(firstChromosomeChild.Str), firstChromosomeChild.Fitness);
        population.Add(firstChromosomeChild);
    }


    str = string.Concat(secondChromosomeParent.Str.Substring(0, random), firstChromosomeParent.Str.Substring(random));
    fitness = Fitness(str);
    var secondChromosomeChild = new Chromosome
    {
        Str = str,
        Fitness = fitness
    };

    Mutate(ref secondChromosomeChild);

    if (!population.Contains(secondChromosomeChild))
    {
        Console.WriteLine("{0} - {1}", ConvertToRTL(secondChromosomeChild.Str), secondChromosomeChild.Fitness);
        population.Add(secondChromosomeChild);
    }
}

string ConvertToRTL(string str)
{
    StringBuilder stringBuilder = new();
    for (int i = str.Length - 1; i >= 0; i--)
        stringBuilder.Append(str[i]);
    return stringBuilder.ToString();
}

void Mutate(ref Chromosome chromosome)
{
    double probability = Random.Shared.NextDouble();
    if (probability < 0.3)
    {
        for (int i = 0; i < target.Length; i++)
            if (chromosome.Str[i] != target[i])
            {
                var random = Random.Shared.Next(0, target.Length);
                var characters = chromosome.Str.ToCharArray();
                characters[i] = target[random];
                chromosome.Str = new string(characters);
                break;
            }
    }
    chromosome.Str.ToCharArray();
}

Console.OutputEncoding = Encoding.UTF8;
CreatePopulation();
Stopwatch stopwatch = new();
stopwatch.Start();

while (true)
{
    CrossOver();
    bool found = population.OrderByDescending(chromosome => chromosome.Fitness).First().Str == target;
    if (found)
    {
        stopwatch.Stop();

        Console.WriteLine("Found in {0}", stopwatch.Elapsed);
        break;
    }
}

struct Chromosome
{
    public string Str { get; set; }
    public int Fitness { get; set; }
}