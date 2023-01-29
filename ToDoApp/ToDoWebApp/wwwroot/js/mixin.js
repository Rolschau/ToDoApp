// Global Mixins. The methods are available on all components.
Vue.mixin({
    data() {
        return {
            language: 'da',
            //isMobile: window.matchMedia('(max-width: 425px)').matches,
        }
    },
    methods: {
        LocalizedString(value) {
            return value?.toLocaleString(this.language);
        },
    },
});