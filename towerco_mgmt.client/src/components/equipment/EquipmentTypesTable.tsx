import type { EquipmentTypeSummary } from '@/types';

function EquipmentTypesTable({ types }: { types: EquipmentTypeSummary[] }) {
    return (
        <table className="table">
            <thead><tr><th>Type</th><th>Category</th><th>Items in service</th></tr></thead>
            <tbody>
                {types.map(t => <tr key={t.id}><td>{t.name}</td><td>{t.category}</td><td>{t.itemCount}</td></tr>)}
            </tbody>
        </table>
    );
}

export default EquipmentTypesTable;
