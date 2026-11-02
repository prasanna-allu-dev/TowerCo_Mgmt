import { Card, StatusBadge } from '@/shared/components';
import type { SiteDetail } from '@/features/sites/types';

function SiteInfoCard({ site }: { site: SiteDetail }) {
    return (
        <Card title="Site">
            <dl className="details">
                <dt>Code</dt><dd className="mono">{site.siteCode}</dd>
                <dt>Status</dt><dd><StatusBadge status={site.status} /></dd>
                <dt>Location</dt><dd>{site.city}, {site.countryCode}</dd>
                <dt>Coordinates</dt><dd className="mono">{site.latitude}, {site.longitude}</dd>
            </dl>
        </Card>
    );
}

export default SiteInfoCard;
