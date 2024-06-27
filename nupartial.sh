cd Pages;
cat > "$1".cshtml <<EOF
@* @model object  *@

<div>
  <p>lorem ipsum</p>
</div> 

EOF
cd ..;