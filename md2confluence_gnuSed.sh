#!/bin/bash
# ---------------------- replace head ----------------------
h5="h5. "
h4="h4. "
h3="h3. "
h2="h2. "
h1="h1. "

sed -i "s/^#####\( \)/${h5}/" ${1}
sed -i "s/^####\( \)/${h4}/" ${1}
sed -i "s/^###\( \)/${h3}/" ${1}
sed -i "s/^##\( \)/${h2}/" ${1}
sed -i "/${h2}/a ----" ${1}
sed -i "s/^#\( \)/${h1}/" ${1}
sed -i "/${h1}/a ----" ${1}

# ---------------------- replace list ----------------------
l2="-- "
l3="--- "
l4="---- "

sed -i "s/			- /${l4}/" ${1}
sed -i "s/		- /${l3}/" ${1}
sed -i "s/	- /${l2}/" ${1}

# ---------------------- replace bold ----------------------
sed -i "s/\`/\*/g" ${1}
sed -i "s/\*\*/\*/g" ${1}

# ---------------------- replace link ----------------------
sed -i "/\:\/\//{ s/)/\]/ }" ${1}
sed -i "/\:\/\//{ s/\](/\|/ }" ${1}

# ---------------------- replace task ----------------------
sed -i "s/\- \[.\]/\-\- []/g" ${1}

# ---------------------- not modify below ----------------------

# Because there may be one or more # in one line, like ### or #####
# we need to substitute the # according to the order from most to least
# to avoid the wrong substitution like ####h1. 













