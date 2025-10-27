import CloseIcon from '@mui/icons-material/Close';
import DoneIcon from '@mui/icons-material/Done';
import EditIcon from '@mui/icons-material/Edit';
import SaveIcon from '@mui/icons-material/Save';
import ToggleOnIcon from '@mui/icons-material/ToggleOn';
import ToggleOffIcon from '@mui/icons-material/ToggleOff';
import {
    Alert,
    Box,
    CircularProgress,
    IconButton,
    List,
    ListItem,
    ListItemText,
    Paper,
    Snackbar,
    TextField,
    Typography,
    type AlertColor
} from '@mui/material';
import { useEffect, useState } from 'react';
import { fetchGroups, updateGroup, validateGroup } from '../../Services/SettingsService';
import type { IGroup } from '../../Interfaces/Groups/IGroup';
import { ApiError } from '../../Utils/api';

export default function SettingsPage() {

    const [groups, setGroups] = useState<IGroup[]>([]);
    const [loading, setLoading] = useState(true);

    const [editingId, setEditingId] = useState<number | null>(null);
    const [editingValue, setEditingValue] = useState('');
    const [saving, setSaving] = useState(false);
    const [validating, setValidating] = useState(false);
    const [toggling, setToggling] = useState<number | null>(null);

    const [snackOpen, setSnackOpen] = useState(false);
    const [snackSeverity, setSnackSeverity] = useState<AlertColor>('info'); // 'success' | 'error' | 'info' | 'warning'
    const [snackMessage, setSnackMessage] = useState('');

    useEffect(() => {
        const controller = new AbortController();

        const loadData = async () => {
            try {
                setLoading(true);
                const data = await fetchGroups(controller.signal);
                setGroups(data);
            } catch (err) {
                let message = 'Ошибка загрузки списка групп';

                if (err instanceof ApiError) {
                    message = err.message;
                }

                showSnack(message, 'error');
            } finally {
                setLoading(false);
            }
        };

        loadData();

        return () => { controller.abort(); };
    }, []);

    const showSnack = (message: string, severity: AlertColor = 'info') => {
        setSnackMessage(message);
        setSnackSeverity(severity);
        setSnackOpen(true);
    };

    const startEdit = (group: IGroup) => {
        setEditingId(group.id);
        setEditingValue(group.internalId ?? '');
    };

    const cancelEdit = () => {
        setEditingId(null);
        setEditingValue('');
    };

    const saveEdit = async (groupId: number) => {
        setSaving(true);
        try {
            const updated = await updateGroup(groupId, { internalId: editingValue });
            setGroups(prev => prev.map(g => g.id === groupId ? { ...g, ...updated } : g));
            setEditingId(null);
            setEditingValue('');
            showSnack('Изменения сохранены', 'success');
        } catch (err) {
            let message = 'Не удалось сохранить изменения';

            if (err instanceof ApiError) {
                message = err.message;
            }

            showSnack(message, 'error');
        } finally {
            setSaving(false);
        }
    };

    const checkEdit = async () => {
        setValidating(true);
        try {
            const result = await validateGroup(editingValue);
            if (result && result.valid) {
                showSnack('Проверка пройдена', 'success');
            } else {
                showSnack(result?.message || 'Проверка не пройдена', 'warning');
            }
        } catch (err) {

            if (err instanceof ApiError && err.details?.aborted) {
                console.log('Запрос был отменен (обновление страницы). Это нормально.');
                return;
            }

            let message = 'Группа не найдена';

            if (err instanceof ApiError) {
                message = err.message;
            }

            showSnack(message, 'error');
        } finally {
            setValidating(false);
        }
    };

    const toggleActive = async (group: IGroup) => {
        setToggling(group.id);
        setGroups(prev => prev.map(g => g.id === group.id ? { ...g, active: !g.isActive } : g));

        try {
            const updated = await updateGroup(group.id, { isActive: !group.isActive });
            setGroups(prev => prev.map(g => g.id === group.id ? { ...g, ...updated } : g));
            showSnack(updated.isActive ? 'Группа активирована' : 'Группа деактивирована', 'success');
        } catch (err) {
            setGroups(prev => prev.map(g => g.id === group.id ? { ...g, active: group.isActive } : g));

            let message = 'Не удалось изменить состояние группы';
            if (err instanceof ApiError)
                message = err.message;
            showSnack(message, 'error');
        } finally {
            setToggling(null);
        }
    };

    if (loading) {
        return (
            <Box sx={{ display: 'flex', justifyContent: 'center', my: 4 }}>
                <CircularProgress />
            </Box>
        );
    }

    return (
        <>
            <Paper
                elevation={3}
                sx={{
                    p: { xs: 2, md: 4 },
                    width: '100%',
                    maxWidth: 900,
                    mx: 'auto',
                    textAlign: 'left'
                }}
            >
                <Typography variant="h4" gutterBottom>
                    Настройки (Отслеживаемые сообщества)
                </Typography>

                <List>
                    {groups.map((group: IGroup) => {
                        const isEditing = editingId === group.id;
                        const isTogglingThis = toggling === group.id;

                        return (
                            <ListItem key={group.id} divider
                                secondaryAction={
                                    isEditing ? (
                                        <Box sx={{ display: 'flex', gap: 1 }}>
                                            <IconButton
                                                edge="end"
                                                aria-label="check"
                                                onClick={checkEdit}
                                                disabled={validating || saving}
                                                color="primary"
                                                size="large"
                                            >
                                                <DoneIcon />
                                            </IconButton>

                                            <IconButton
                                                edge="end"
                                                aria-label="save"
                                                onClick={() => saveEdit(group.id)}
                                                disabled={saving || validating}
                                                color="success"
                                                size="large"
                                            >
                                                <SaveIcon />
                                            </IconButton>

                                            <IconButton
                                                edge="end"
                                                aria-label="cancel"
                                                onClick={cancelEdit}
                                                disabled={saving || validating}
                                                color="inherit"
                                                size="large"
                                            >
                                                <CloseIcon />
                                            </IconButton>
                                        </Box>
                                    ) : (
                                        <Box>
                                            <IconButton
                                                edge="end"
                                                aria-label={group.isActive ? 'deactivate' : 'activate'}
                                                onClick={() => toggleActive(group)}
                                                disabled={isTogglingThis}
                                                size="large"
                                                color={group.isActive ? 'success' : 'default'}
                                            >
                                                {isTogglingThis ? (
                                                    <CircularProgress size={20} />
                                                ) : group.isActive ? (
                                                    <ToggleOnIcon />
                                                ) : (
                                                    <ToggleOffIcon />
                                                )}
                                            </IconButton>

                                            <IconButton
                                                edge="end"
                                                aria-label="edit"
                                                onClick={() => startEdit(group)}
                                                size="large"
                                            >
                                                <EditIcon />
                                            </IconButton>
                                        </Box>
                                    )
                                }
                            >
                                {isEditing ? (
                                    <TextField
                                        fullWidth
                                        value={editingValue}
                                        onChange={(e) => setEditingValue(e.target.value)}
                                        disabled={saving || validating}
                                        variant="standard"
                                        autoFocus
                                        helperText={editingValue.trim() === '' ? 'Название не может быть пустым' : ''}
                                        error={editingValue.trim() === ''}
                                    />
                                ) : (
                                    <ListItemText primary={group.internalId} />
                                )}
                            </ListItem>
                        );
                    })}
                </List>
            </Paper>

            <Snackbar
                open={snackOpen}
                autoHideDuration={4000}
                onClose={() => setSnackOpen(false)}
                anchorOrigin={{ vertical: 'bottom', horizontal: 'center' }}
            >
                <Alert onClose={() => setSnackOpen(false)} severity={snackSeverity} sx={{ width: '100%' }}>
                    {snackMessage}
                </Alert>
            </Snackbar>
        </>
    );
}