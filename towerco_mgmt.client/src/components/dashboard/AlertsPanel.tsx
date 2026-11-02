import { Link } from 'react-router-dom';
import type { DashboardAlert } from '@/types';
import SeverityPill from './SeverityPill';

function AlertRow({ alert }: { alert: DashboardAlert }) {
    return (
        <li className={`alert-row tone-${alert.severity}`}>
            <SeverityPill severity={alert.severity} />
            <div className="alert-text">
                <div className="alert-title">{alert.title}</div>
                <div className="muted">{alert.detail}</div>
            </div>
            {alert.siteId && <Link className="alert-link" to={`/sites/${alert.siteId}`}>Open site</Link>}
        </li>
    );
}

function AlertsPanel({ alerts }: { alerts: DashboardAlert[] }) {
    if (!alerts.length) return <p className="muted">All clear. No indicators need attention.</p>;
    return <ul className="alert-list">{alerts.map(a => <AlertRow key={a.id} alert={a} />)}</ul>;
}

export default AlertsPanel;
