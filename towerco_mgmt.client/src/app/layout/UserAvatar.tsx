import { useState } from 'react';
import { CURRENT_USER, toInitials } from '@/app/currentUser';

function UserAvatar() {
    const [photoFailed, setPhotoFailed] = useState(false);
    if (photoFailed) return <span className="user-avatar" aria-hidden="true">{toInitials(CURRENT_USER.name)}</span>;
    return <img className="user-avatar" src={CURRENT_USER.photoUrl} alt="" onError={() => setPhotoFailed(true)} />;
}

export default UserAvatar;
