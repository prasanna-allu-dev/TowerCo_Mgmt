import { Link } from 'react-router-dom';
import { Pagination, SeverityPill } from '@/shared/components';
import { usePagination } from '@/shared/hooks/usePagination';
import type { EquipmentItem, EquipmentStatus } from '@/features/equipment/types';
import type { Severity } from '@/shared/types/domain';
import { sitePath } from '@/shared/constants/routes';

const STATUS_SEVERITY: Record<EquipmentStatus, Severity> = { Installed: 'ok', Planned: 'warning', Removed: 'neutral' };

interface EquipmentTableProps {
    items: EquipmentItem[];
    showSite?: boolean;
}

function EquipmentRow({ item, showSite }: { item: EquipmentItem; showSite: boolean }) {
    return (
        <tr>
            {showSite && <td><Link to={sitePath(item.siteId)} className="mono">{item.siteCode}</Link></td>}
            <td>{item.typeName}</td>
            <td>{item.category}</td>
            <td>{item.ownerName}</td>
            <td className="mono">{item.serialNumber}</td>
            <td className="mono">{item.installedOn}</td>
            <td>{item.mountHeightMetres === null ? '-' : `${item.mountHeightMetres} m`}</td>
            <td><SeverityPill severity={STATUS_SEVERITY[item.status]} label={item.status} /></td>
        </tr>
    );
}

function EquipmentTable({ items, showSite = false }: EquipmentTableProps) {
    const paging = usePagination(items);
    return (
        <>
        <table className="table">
            <thead>
                <tr>
                    {showSite && <th>Site</th>}
                    <th>Type</th><th>Category</th><th>Owner</th><th>Serial</th><th>Installed</th><th>Mount height</th><th>Status</th>
                </tr>
            </thead>
            <tbody>{paging.pageRows.map(item => <EquipmentRow key={item.id} item={item} showSite={showSite} />)}</tbody>
        </table>
        <Pagination {...paging} />
        </>
    );
}

export default EquipmentTable;
