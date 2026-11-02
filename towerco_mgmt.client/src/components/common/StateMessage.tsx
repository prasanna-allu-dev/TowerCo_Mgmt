interface StateMessageProps {
    loading?: boolean;
    error?: string;
    empty?: boolean;
    emptyText?: string;
}

function StateMessage({ loading, error, empty, emptyText }: StateMessageProps) {
    if (loading) return <p className="muted">Loading...</p>;
    if (error) return <p className="alert-error">{error}</p>;
    if (empty) return <p className="muted">{emptyText}</p>;
    return null;
}

export default StateMessage;
