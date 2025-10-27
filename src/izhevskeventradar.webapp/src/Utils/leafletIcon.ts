import L from 'leaflet';
import icon from 'leaflet/dist/images/marker-icon.png';
import iconShadow from 'leaflet/dist/images/marker-shadow.png';

// Эта магия исправляет пути к иконкам по умолчанию
const DefaultIcon = L.icon({
    iconUrl: icon,
    shadowUrl: iconShadow,
    iconAnchor: [12, 41], // Точка "якоря" иконки
    popupAnchor: [1, -34], // Точка "якоря" для popup
});

L.Marker.prototype.options.icon = DefaultIcon;