public class Friendship(Person person, Person person1, int weight) : IEdge<Person>
{
    public Person Source {get; } = person;
    public Person Target {get; } = person1;
    public double Weight {get; } = weight;
    public string Diversión { get; set; }
}