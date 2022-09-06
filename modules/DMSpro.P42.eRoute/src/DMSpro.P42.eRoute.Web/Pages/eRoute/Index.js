$(() => {
    const markerUrl = 'https://js.devexpress.com/Demos/RealtorApp/images/map-marker.png';

    const markersData = [
        {
            location: [40.755833, -73.986389],
            tooltip: {
                text: 'SM001',
            },
        }, {
            location: '40.7825, -73.966111',
            tooltip: {
                text: 'SM002',
            },
        }, {
            location: { lat: 40.753889, lng: -73.981389 },
            tooltip: {
                text: 'SM003',
            },
        }, {
            location: 'Brooklyn Bridge,New York,NY',
            tooltip: {
                text: 'Brooklyn Bridge',
            },
        },
    ];

    const locationsData = [
        [40.782500, -73.966111],
        [40.755833, -73.986389],
        [40.753889, -73.981389],
        'Brooklyn Bridge,New York,NY',
    ];

    const locationsDataReal = [
        [40.755833, -73.986389],
        'Brooklyn Bridge,New York,NY',
        [40.782500, -73.966111],
        [40.753889, -73.981389],
    ];

    const encodeMessage = (marker) => {
        // ...
        // Encode the `message` string with your favorite sanitizing tool
        // ...
        marker = "<div class='marker-tooltip' style='color: red; width: 100px;'> Hello "
            + marker.tooltip.text
            + " /" + marker.location
            + "</div>";
        return marker;
    };

    const sanitizeMarkers = (markers) => {
        markers.forEach(marker => {
            marker.tooltip.text = encodeMessage(marker);
        })
        return markers;
    };

    const modeTypes = [
        {
            key: 'driving',
            name: 'Lái xe',
        }, {
            key: 'walking',
            name: 'Đi bộ',
        }
    ];
    let curModeType = 'driving';

    const routesCollection = [{
        locations: locationsData
    }, {
        locations: locationsDataReal
    }];
    let curRoute = routesCollection[0];

    const map = $('#map').dxMap({
        provider: 'bing',
        apiKey: {
            // Specify your API keys for each map provider:
            // bing: "YOUR_BING_MAPS_API_KEY",
            //google: "AIzaSyAZqFx72Ot_fgqzEB1YBVCjS5M4alZyf1c",
            // googleStatic: "YOUR_GOOGLE_STATIC_MAPS_API_KEY"
        },
        zoom: 14,
        height: '70vh',
        width: '100%',
        controls: true,
        //markerIconSrc: markerUrl,
        markers: sanitizeMarkers(markersData),
        center: {
            lat: 16.596677167643488,
            lng: 107.11245681292728
        },
        /*activeStateEnabled: true,
        elementAttr: {
            id: "elementId",
            class: "class-name"
        },*/
        onClick: function (e) {
            console.log(e.location);
            e.component.addMarker({
                // Location of the clicked point on the map
                location: e.location,
                iconSrc: markerUrl,
                tooltip: {
                    text: e.location.lat + ' / ' + e.location.lng,
                },
            });
        },
        onDisposing: null,
        onInitialized: null,
        onMarkerAdded: null,
        onMarkerRemoved: null,
        onOptionChanged: null,
        onReady: null,
        onRouteAdded: null,
        onRouteRemoved: null,
        routes: [curRoute],
    }).dxMap('instance');

    $('#choose-mode').dxSelectBox({
        dataSource: modeTypes,
        displayExpr: 'name',
        valueExpr: 'key',
        value: 'driving',
        onValueChanged(data) {
            //map.removeRoute(curRoute);
            curModeType = data.value;
            curRoute = $.extend({}, curRoute, { mode: data.value, });
            map.option('routes', [curRoute]);
        },
    });

    $('#choose-color').dxSelectBox({
        dataSource: ['blue', 'green', 'red'],
        value: 'blue',
        onValueChanged(data) {
            map.option('routes', [$.extend({}, map.option('routes')[0], {
                color: data.value,
            })]);
            //map.option('provider', 'bing');
        },
    });

    $('#use-custom-routes').dxCheckBox({
        value: true,
        text: 'Change route',
        onValueChanged(data) {
            console.log(curRoute);
            var routeAdd = data.value ? 0 : 1;
            map.removeRoute(curRoute);
            curRoute = $.extend({}, routesCollection[routeAdd], { mode: curModeType, });
            map.addRoute(curRoute);
        },
    });
});
