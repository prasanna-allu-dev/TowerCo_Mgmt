import { ROUTES, sitePath } from '@/shared/constants/routes';
import { Link, useNavigate } from 'react-router-dom';
import { towersApi } from '@/features/configuration/api';
import { CapacityBar, Card, Pagination, StateMessage, StatusBadge } from '@/shared/components';
import { useApi } from '@/shared/hooks/useApi';
import { usePagination } from '@/shared/hooks/usePagination';
import PageHeader from '@/shared/components/PageHeader';
import { countLabel } from '@/shared/utils/format';
import type { TowerConfiguration } from '@/features/configuration/types';

function ConfigurationRow({ tower }: { tower: TowerConfiguration }) {
    const navigate = useNavigate();
    return (
        <tr className="clickable" onClick={() => navigate(sitePath(tower.siteId))}>
            <td className="mono">{tower.siteCode}</td>
            <td>{tower.siteName}</td>
            <td><StatusBadge status={tower.siteStatus} /></td>
            <td>{tower.towerType}</td>
            <td>{tower.heightMetres} m</td>
            <td>{tower.capacity.maxTenants}</td>
            <td><CapacityBar capacity={tower.capacity} /></td>
        </tr>
    );
}

function ConfigurationTable({ towers }: { towers: TowerConfiguration[] }) {
    const paging = usePagination(towers);
    return (
        <>
            <table className="table">
                <thead><tr><th>Site</th><th>Name</th><th>Status</th><th>Type</th><th>Height</th><th>Max tenants</th><th>Utilisation</th></tr></thead>
                <tbody>{paging.pageRows.map(t => <ConfigurationRow key={t.towerId} tower={t} />)}</tbody>
            </table>
            <Pagination {...paging} />
        </>
    );
}

function ConfigurationPage() {
    const { data, error, loading } = useApi(() => towersApi.list());

    return (
        <>
            <PageHeader
                title="Tower configuration"
                subtitle="Tower type, height and tenant slots per site"
                actions={<Link className="btn btn-primary" to={ROUTES.newTowerSite}>+ New tower site</Link>}
            />
            <Card title="Towers" subtitle={countLabel(data?.length, 'tower', 'towers')}>
                <StateMessage loading={loading} error={error} empty={data?.length === 0} emptyText="No towers configured." />
                {!!data?.length && <ConfigurationTable towers={data} />}
            </Card>
        </>
    );
}

export default ConfigurationPage;
