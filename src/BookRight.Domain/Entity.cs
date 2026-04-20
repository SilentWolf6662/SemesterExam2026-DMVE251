namespace BookRight.Domain
{
    /// <summary>
    /// Base class for alle Entities. En Entity har en unik identitet (Id).
    /// To entities er ens hvis de har samme Id.
    /// </summary>
    public abstract class Entity
    {
        public Guid Id { get; protected set; }
    }
}