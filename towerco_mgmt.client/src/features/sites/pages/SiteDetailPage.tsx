import { Link, useParams } from 'react-router-dom';
import { equipmentApi } from '@/features/equipment/api';
import { healthApi } from '@/features/health/api';
import { sitesApi } from '@/features/sites/api';
import { Card, StateMessage } from '@/shared/components';
import { SiteEquipmentCard } from '@/features/equipment/components';
import { SiteHealthCard } from '@/features/health/components';
import { SiteInfoCard, TenanciesTable, TowerCapacityCard } from '@/features/sites/components';
import { useApi } from '@/shared/hooks/useApi';
import PageHeader from '@/shared/components/PageHeader';
import { ROUTES } from '@/shared/constants/routes';

function SiteDetailPage() {
    const siteId = Number(useParams().siteId);
    const { data: site, error, loading } = useApi(() => sitesApi.get(siteId), [siteId]);
    const equipment = useApi(() => equipmentApi.list({ siteId }), [siteId]);
    const health = useApi(() => healthApi.site(siteId), [siteId]);

    return (
        <>
            <Link to={ROUTES.sites} className="back-link">← Back to sites</Link>
            <PageHeader title={site?.name ?? 'Site'} subtitle="Site, tower, capacity and tenancies" />
            <StateMessage loading={loading} error={error} />
            {site && (
                <>
                    <div className="grid-2"><SiteInfoCard site={site} /><TowerCapacityCard site={site} /></div>
                    <Card title="Tenancies">
                        <StateMessage empty={!site.tenancies.length} emptyText="No tenants on this tower yet." />
                        {!!site.tenancies.length && <TenanciesTable tenancies={site.tenancies} />}
                    </Card>
                    <StateMessage loading={equipment.loading} error={equipment.error} />
                    {equipment.data && <SiteEquipmentCard items={equipment.data} />}
                    {health.data && <SiteHealthCard health={health.data} />}
                </>
            )}
        </>
    );
}

export default SiteDetailPage;
