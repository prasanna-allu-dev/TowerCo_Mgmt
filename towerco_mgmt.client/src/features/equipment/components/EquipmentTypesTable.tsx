import type { EquipmentTypeSummary } from '@/features/equipment/types';
import { Pagination } from '@/shared/components';
import { usePagination } from '@/shared/hooks/usePagination';

function EquipmentTypesTable({ types }: { types: EquipmentTypeSummary[] }) {
    const paging = usePagination(types);
    return (
        <>
        <table className="table">
            <thead><tr><th>Type</th><th>Category</th><th>Items in service</th></tr></thead>
            <tbody>
                {paging.pageRows.map(t => <tr key={t.id}><td>{t.name}</td><td>{t.category}</td><td>{t.itemCount}</td></tr>)}
            </tbody>
        </table>
        <Pagination {...paging} />
        </>
    );
}

export default EquipmentTypesTable;
