namespace BookRight.Domain;

/// <summary>
/// Base class for alle Entities. En Entity har en unik identitet (Id).
/// To entities er ens hvis de har samme Id.
/// </summary>
public abstract class Entity
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
}

/// <summary>
/// Base class for Aggregate Roots.
///
/// Saadan identificerer man en Aggregate Root:
///   1. Egen livscyklus — kan oprettes/slettes uafhaengigt
///   2. Transaktionsgraense — aendringer gemmes som en enhed
///   3. Eget repository — hentes direkte fra databasen
///   4. Refereres via ID — andre aggregater holder kun FK
/// </summary>
public abstract class AggregateRoot : Entity
{
}