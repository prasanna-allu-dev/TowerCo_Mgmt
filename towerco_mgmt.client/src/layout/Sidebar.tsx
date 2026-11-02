import { NavLink } from 'react-router-dom';
import { APP_NAME, APP_SUBTITLE, NAV_GROUPS } from '@/constants/navigation';
import type { NavGroup, NavItem } from '@/constants/navigation';

function NavEntry({ item }: { item: NavItem }) {
    if (item.disabled) return <span className="nav-link disabled" title="Coming soon">{item.icon} {item.label}</span>;
    return <NavLink to={item.path} className="nav-link">{item.icon} {item.label}</NavLink>;
}

function NavSection({ group }: { group: NavGroup }) {
    return (
        <div className="nav-group" role="group" aria-label={group.label}>
            <div className="nav-group-label">{group.label}</div>
            {group.items.map(item => <NavEntry key={item.path} item={item} />)}
        </div>
    );
}

function Sidebar() {
    return (
        <aside className="sidebar">
            <div className="brand">
                <span className="brand-mark">T</span>
                <div>
                    <div className="brand-name">{APP_NAME}</div>
                    <div className="brand-sub">{APP_SUBTITLE}</div>
                </div>
            </div>
            <nav className="nav">
                {NAV_GROUPS.map(group => <NavSection key={group.label} group={group} />)}
            </nav>
        </aside>
    );
}

export default Sidebar;
