
/** 
 Search form
 */
document.addEventListener("DOMContentLoaded", function () {
    const searchContainer = document.querySelector(".search-container");
    const searchIcon = document.querySelector(".search-icon");
    const searchInput = document.querySelector(".search-input");

    if (searchContainer && searchIcon && searchInput) {
        searchIcon.addEventListener("click", function (event) {
            event.stopPropagation();
            searchContainer.classList.add("active");
            searchInput.focus();
        });

        document.addEventListener("click", function (event) {
            if (!searchContainer.contains(event.target)) {
                searchContainer.classList.remove("active");
            }
        });
    }
});